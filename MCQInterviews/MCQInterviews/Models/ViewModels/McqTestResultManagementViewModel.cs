namespace MCQInterviews.Models.ViewModels
{
    public class McqTestResultManagementViewModel
    {
        public string UserName { get; set; }
        public string Country { get; set; }
        public string TestTitle { get; set; }
        public string JobCategory { get; set; }
        public string JobTitle { get; set; }
        public string JobLevel { get; set; }
        public int Score { get; set; }
        public DateTime DateTaken { get; set; }
    }
}
