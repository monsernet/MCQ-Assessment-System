using AuthSystem.Areas.Identity.Data;

namespace MCQInterviews.Models.Domain
{
    public class VoiceSample
    {
        public int Id { get; set; }

        // Foreign key property
        public string CandidateId { get; set; }

        // Navigation property
        public ApplicationUser Candidate { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedOn { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public string AdminFeedback { get; set; }
    }
}
