namespace backend.Models
{
    public class Document
    {
        public int DocumentId { get; set; }
        public int CaseId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string? Description { get; set; }

		// Navigation properties
        public Case Case { get; set; } = null!;
	}
}
