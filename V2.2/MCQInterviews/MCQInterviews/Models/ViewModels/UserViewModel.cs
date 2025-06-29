using AuthSystem.Areas.Identity.Data;

namespace MCQInterviews.Models.ViewModels
{
    public class UserViewModel
    {
        public ApplicationUser User { get; set; }
        public int NumberOfLogins { get; set; }
    }
}
