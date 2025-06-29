namespace MCQInterviews.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalJobCategories { get; set; }
        public int TotalJobTitles { get; set; }
        public int TotalJobLevels { get; set; }
        public int TotalMCQTests { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalTestPassed { get; set; }
        public int TotalDiffTypes { get; set; }
        public List<UserViewModel> Users { get; set; }
        public List<ThemeManagementViewModel> JobCategories { get; set; }
        public List<JobTitleManagementViewModel> JobTitles { get; set; }
        public List<JobLevelManagementViewModel> JobLevels { get; set; }
        public List<McqViewModel> McqTests { get; set; }
        public List<TopUserDetailsViewModel> TopUsersDetails { get; set; }
        public List<DifficultyAllocationViewModel> DifficultyAllocations { get; set; }


    }
}
