using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
		}

		public DbSet<Tenant> Tenants { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Case> Cases { get; set; }

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
				entity.Property(e => e.CaseTitle).IsRequired().HasMaxLength(200);
				entity.Property(e => e.CaseNumber).IsRequired().HasMaxLength(50);
				
			});
		}
	}
}
