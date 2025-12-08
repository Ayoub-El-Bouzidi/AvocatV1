using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Tenant> Tenants { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Case> Cases { get; set; }
		public DbSet<CaseClient> CaseClients { get; set; } // ✅ Add this

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure Tenant
			modelBuilder.Entity<Tenant>(entity =>
			{
				entity.HasKey(e => e.TenantId);
				entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
			});

			// Configure Client
			modelBuilder.Entity<Client>(entity =>
			{
				entity.HasKey(e => e.ClientId);
				entity.Property(e => e.Email).IsRequired().HasMaxLength(100);

				entity.HasOne(e => e.Tenant)
					.WithMany(t => t.Clients)
					.HasForeignKey(e => e.TenantId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Case
			modelBuilder.Entity<Case>(entity =>
			{
				entity.HasKey(e => e.CaseId);
				entity.Property(e => e.CaseNumber).IsRequired().HasMaxLength(50);
				entity.Property(e => e.CaseTitle).IsRequired().HasMaxLength(300);

				entity.HasOne(e => e.Tenant)
					.WithMany(t => t.Cases)
					.HasForeignKey(e => e.TenantId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasIndex(e => e.CaseNumber).IsUnique();
			});

			// ✅ Configure CaseClient (Many-to-Many)
			modelBuilder.Entity<CaseClient>(entity =>
			{
				entity.HasKey(e => e.CaseClientId);

				// Case → CaseClients relationship
				entity.HasOne(e => e.Case)
					.WithMany(c => c.CaseClients)
					.HasForeignKey(e => e.CaseId)
					.OnDelete(DeleteBehavior.Cascade);

				// Client → CaseClients relationship
				entity.HasOne(e => e.Client)
					.WithMany(c => c.CaseClients)
					.HasForeignKey(e => e.ClientId)
					.OnDelete(DeleteBehavior.Cascade);

				// Prevent duplicate client on same case
				entity.HasIndex(e => new { e.CaseId, e.ClientId }).IsUnique();
			});

			// Seed initial data
			modelBuilder.Entity<Tenant>().HasData(
				new Tenant
				{
					TenantId = 1,
					CompanyName = "Demo Law Firm",
					Email = "demo@lawfirm.com",
					Phone = "+212-123-456-789",
					SubscriptionPlan = "Professional",
					SubscriptionStatus = "Active",
					SubscriptionStartDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), // ✅ Static date
					IsActive = true,
					CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
				}
			);
			modelBuilder.Entity<ApplicationUser>(entity =>
			{
				entity.HasOne(e => e.Tenant)
					.WithMany()
					.HasForeignKey(e => e.TenantId)
					.OnDelete(DeleteBehavior.Restrict);
			});
		}
	}
}