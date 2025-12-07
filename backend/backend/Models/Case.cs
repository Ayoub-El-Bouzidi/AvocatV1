namespace backend.Models
{
    public class Case
    {
        public int CaseId { get; set; }
        public int TenantId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string CaseTitle { get; set; } = string.Empty;
        public string? CaseType { get; set; } // Civil, Legal, Medical, Technical, Other
        public string? CourtName { get; set; }
        public string? JudgeName { get; set; }
        public string CaseStatus { get; set; } = "Open"; // Open, Closed, Pending, On Hold
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent
		public DateTime OpenDate { get; set; } = DateTime.UtcNow;
		public DateTime? CloseDate { get; set; }
		public string? Description { get; set; }
		public string? Notes { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public DateTime? ModifiedDate { get; set; }

		// Navigation properties
		public Tenant Tenant { get; set; } = null!;
        public ICollection<CaseClient> CaseClients { get; set; } = new List<CaseClient>();
	}
}
