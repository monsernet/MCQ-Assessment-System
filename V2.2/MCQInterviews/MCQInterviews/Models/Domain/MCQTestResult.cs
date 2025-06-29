using AuthSystem.Areas.Identity.Data;

namespace MCQInterviews.Models.Domain
{
    public class MCQTestResult
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to ApplicationUser
        public int MCQId { get; set; } // Foreign key to MCQ
        public int Score { get; set; }
        public int Points { get; set; }
        public DateTime DateTaken { get; set; }


        // Navigation properties
        public ApplicationUser User { get; set; } // Navigation property to ApplicationUser
        public MCQ MCQ { get; set; } // Navigation property to MCQ
    }
}
