namespace MCQInterviews.Models.ViewModels
{
    public class ThemeManagementViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalJobTitles { get; set; }
        public int TotalMCQTests { get; set; }
        public int TotalTestsPassed { get; set; }
    }
}
