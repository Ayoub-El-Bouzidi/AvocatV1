namespace backend.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public int UserId { get; set; } = string.Empty
        //public string ClintType { get; set; } = "Individual"; // Individual, Company
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        //public string? CompanyName { get; set; }
		public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Adresse { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }

		// Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Case> Cases { get; set; } = new List<Case>();
	}
}
