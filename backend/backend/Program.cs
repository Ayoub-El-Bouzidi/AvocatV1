using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. SENTRY - Error Tracking (Already there ✓)
// ============================================
// WHY: Tracks errors in production so you know when something breaks
builder.WebHost.UseSentry(o =>
{
	o.Dsn = "https://175f92420ca43cbbc0506ed126d858f6@o4510490789543936.ingest.de.sentry.io/4510490791706704";
	o.Debug = true; // Set to false in production
});

// ============================================
// 2. DATABASE - Entity Framework Core
// ============================================
// WHY: Connects your app to SQL Server database
// This lets you save/read Clients, Cases, Documents, etc.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		sqlOptions => sqlOptions.EnableRetryOnFailure() // Auto-retry on connection issues
	));

// ============================================
// 3. IDENTITY - User Authentication
// ============================================
// WHY: Manages user login, registration, passwords, roles
// Essential for multi-user SaaS where lawyers/clients need accounts
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
	// Password requirements
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 8;

	// Lockout settings (security)
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
	options.Lockout.MaxFailedAccessAttempts = 5;

	// User settings
	options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ============================================
// 4. COOKIE AUTHENTICATION SETTINGS
// ============================================
// WHY: Controls login cookie behavior
builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Account/Login"; // Where to redirect if not logged in
	options.LogoutPath = "/Account/Logout";
	options.AccessDeniedPath = "/Account/AccessDenied";
	options.ExpireTimeSpan = TimeSpan.FromHours(8); // Login expires after 8 hours
	options.SlidingExpiration = true; // Reset timer on activity
});

// ============================================
// 5. SESSION - Store temporary data
// ============================================
// WHY: Store user data during their visit (like shopping cart)
// Useful for multi-step forms, temporary data
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// ============================================
// 6. DISTRIBUTED CACHE - Performance
// ============================================
// WHY: Speeds up your app by caching frequently used data
// Start with memory cache, later upgrade to Redis for production
builder.Services.AddDistributedMemoryCache();

// ============================================
// 7. HTTP CONTEXT ACCESSOR - Get current user
// ============================================
// WHY: Allows you to get current logged-in user anywhere in your code
// Needed for multi-tenancy (to know which law firm the user belongs to)
builder.Services.AddHttpContextAccessor();

// ============================================
// 8. CONTROLLERS WITH VIEWS (Already there ✓)
// ============================================
builder.Services.AddControllersWithViews()
	.AddJsonOptions(options =>
	{
		// Handle circular references in JSON (Case → Client → Case)
		options.JsonSerializerOptions.ReferenceHandler =
			System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
	});

// ============================================
// 9. REPOSITORIES - Clean Code Pattern
// ============================================
// WHY: Separates database logic from controllers
// Makes code testable and maintainable
// We'll add these as you create each repository
// Example: builder.Services.AddScoped<IClientRepository, ClientRepository>();

// ============================================
// 10. AUTO MAPPER (Optional but recommended)
// ============================================
// WHY: Automatically converts between Models and DTOs
// Saves you from writing repetitive mapping code
// builder.Services.AddAutoMapper(typeof(Program));

// ============================================
// 11. LOGGING - Track what happens
// ============================================
// WHY: Helps debug issues and understand user behavior
builder.Services.AddLogging(logging =>
{
	logging.AddConsole();
	logging.AddDebug();
    //logging.AddFilter("Logs/app-{Date}.txt"); // Requires Serilog.Extensions.Logging.File
});

// ============================================
// 12. AUTHORIZATION POLICIES - Control access
// ============================================
// WHY: Define who can do what (Admin vs Lawyer vs Client)
builder.Services.AddAuthorization(options =>
{
	// Only Admins can access admin panel
	options.AddPolicy("AdminOnly", policy =>
		policy.RequireRole("Admin"));

	// Only Lawyers can create cases
	options.AddPolicy("LawyerOnly", policy =>
		policy.RequireRole("Admin", "Lawyer"));

	// Must belong to same tenant (law firm)
	options.AddPolicy("SameTenant", policy =>
		policy.RequireAssertion(context =>
		{
			// We'll implement this check later
			return true;
		}));
});

var app = builder.Build();

// ============================================
// MIDDLEWARE PIPELINE (Order matters!)
// ============================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}
else
{
	// Show detailed errors in development
	app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

// ============================================
// IMPORTANT: Session must come BEFORE Authentication
// ============================================
app.UseSession(); // Enable session

// ============================================
// Authentication & Authorization
// WHY: Check if user is logged in and has permission
// ============================================
app.UseAuthentication(); // Who are you?
app.UseAuthorization();  // What can you do?

app.MapStaticAssets();

// ============================================
// ROUTES
// ============================================
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

// ============================================
// SEED DATABASE (Run once on startup)
// ============================================
// WHY: Create initial admin user and roles
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
		var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
		await SeedData.Initialize(services, userManager, roleManager);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred seeding the DB.");
	}
}

app.Run();