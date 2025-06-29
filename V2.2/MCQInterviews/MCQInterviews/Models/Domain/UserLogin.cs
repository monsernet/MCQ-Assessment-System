using AuthSystem.Areas.Identity.Data;

namespace MCQInterviews.Models.Domain
{
    public class UserLogin
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime LoginDate { get; set; }

        // Navigation property to ApplicationUser
        public ApplicationUser User { get; set; }
    }
}
