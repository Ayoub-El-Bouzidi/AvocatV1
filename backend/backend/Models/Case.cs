using System.Reflection.Metadata;

namespace backend.Models
{
    public class Case
    {
        public int CaseId { get; set; }
        public string UserId { get; set; } = string.Empty;
		public int ClientId { get; set; }
		public string CaseNumber { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string? CaseType { get; set; }
		public string Status { get; set; } = "Open";
		public string? Description { get; set; }

		public DateTime OpenDate { get; set; } = DateTime.UtcNow;
		public DateTime? CloseDate { get; set; }

		// Navigation properties
		public ApplicationUser User { get; set; } = null!;
        public Client Client { get; set; } = null!;
		public ICollection<Document> Documents { get; set; } = new List<Document>();
	}
}
