using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
	public static class SeedData
	{
		public static async Task Initialize(
			IServiceProvider serviceProvider,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			// Create Roles
			string[] roleNames = { "Admin", "Lawyer", "Paralegal", "Secretary", "Client" };

			foreach (var roleName in roleNames)
			{
				if (!await roleManager.RoleExistsAsync(roleName))
				{
					await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			// Create default admin user
			var adminEmail = "admin@lawfirm.com";
			var adminUser = await userManager.FindByEmailAsync(adminEmail);

			if (adminUser == null)
			{
				var admin = new ApplicationUser
				{
					UserName = adminEmail,
					Email = adminEmail,
					FirstName = "Admin",
					LastName = "User",
					TenantId = 1,
					EmailConfirmed = true,
					IsActive = true,
					CreatedDate = DateTime.UtcNow
				};

				var result = await userManager.CreateAsync(admin, "Admin@123");

				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(admin, "Admin");
				}
			}
		}
	}
}