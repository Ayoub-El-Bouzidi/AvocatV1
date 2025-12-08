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

		//public DbSet<Tenant> Tenants { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Case> Cases { get; set; }
		public DbSet<Document> Documents { get; set; } // ✅ Add this

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Client
			modelBuilder.Entity<Client>(entity =>
			{
				entity.HasKey(e => e.ClientId);
				entity.HasOne(e => e.User)
					.WithMany()
					.HasForeignKey(e => e.UserId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// Case
			modelBuilder.Entity<Case>(entity =>
			{
				entity.HasKey(e => e.CaseId);
				entity.HasIndex(e => e.CaseNumber).IsUnique();

				entity.HasOne(e => e.User)
					.WithMany()
					.HasForeignKey(e => e.UserId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Client)
					.WithMany(c => c.Cases)
					.HasForeignKey(e => e.ClientId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// Document
			modelBuilder.Entity<Document>(entity =>
			{
				entity.HasKey(e => e.DocumentId);
				entity.HasOne(e => e.Case)
					.WithMany(c => c.Documents)
					.HasForeignKey(e => e.CaseId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
