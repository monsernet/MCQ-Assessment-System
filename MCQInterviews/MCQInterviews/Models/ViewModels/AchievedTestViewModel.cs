namespace MCQInterviews.Models.ViewModels
{
    public class AchievedTestViewModel
    {
        public int MCQId { get; set; }
        public string MCQName { get; set; }
        public string JobTitleName { get; set; }
        public string JobLevelName { get; set; }
        public int Score { get; set; }
        public DateTime DateTaken { get; set; }
    }
}
