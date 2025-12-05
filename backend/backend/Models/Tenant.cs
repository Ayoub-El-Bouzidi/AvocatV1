namespace backend.Models
{
    public class Tenant
    {
        public int TenantId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
		public string? Adresse { get; set; }
		public string SubscriptionPlan { get; set; } = "Basic"; // Basic, Professional, Enterprise
		public string SubscriptionStatus { get; set; } = "Active"; // Active, Suspended, Cancelled
		public DateTime SubscriptionStartDate { get; set; }
		public DateTime? SubscriptionEndDate { get; set; }
		public int MaxUsers { get; set; } = 5;
		public int MaxCases { get; set; } = 100;
		public bool IsActive { get; set; } = true;
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public DateTime? ModifiedDate { get; set; }

		// Navigation properties
		public ICollection<Client> Clients { get; set; } = new List<Client>();
		public ICollection<Case> Cases { get; set; } = new List<Case>();
	}
}
