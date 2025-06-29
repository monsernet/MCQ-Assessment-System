namespace MCQInterviews.Models.ViewModels
{
    public class DashboardViewModel
    {
        // User Information
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime JoinDate { get; set; }
        public string Country { get; set; }
        public string ProfileImage { get; set; }

        // Statistics
        public int TotalMCQs { get; set; }
        public int TotalPoints { get; set; }
        public double ProgressPercentage { get; set; }

        // Achievements
        public List<string> Achievements { get; set; }

        // Feedback
        public string Feedback { get; set; }
    }
}
