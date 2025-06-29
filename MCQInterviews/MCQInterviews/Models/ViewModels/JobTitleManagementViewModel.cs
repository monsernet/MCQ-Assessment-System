namespace MCQInterviews.Models.ViewModels
{
    public class JobTitleManagementViewModel
    {
        public int Id { get; set; }
        public string JobTitleName { get; set; }
        public string JobCategoryName { get; set; }
        public int TotalMCQTests { get; set; }
        public int TotalPassedTests { get; set; }

    }
}
