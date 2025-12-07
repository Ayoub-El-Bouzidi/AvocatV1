namespace backend.Models
{
    public class CaseClient
    {
		public int CaseClientId { get; set; }
		public int CaseId { get; set; }
		public int ClientId { get; set; }

		// Additional properties to describe the relationship
		public string ClientRole { get; set; } = "Primary"; // Primary, Co-Client, Plaintiff, Defendant, Witness
		public bool IsPrimaryClient { get; set; } = false; // Mark one client as primary contact
		public DateTime AddedDate { get; set; } = DateTime.UtcNow;
		public string? Notes { get; set; }

		// Navigation properties
		public Case Case { get; set; } = null!;
		public Client Client { get; set; } = null!;
	}
}
