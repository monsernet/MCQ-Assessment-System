namespace MCQInterviews.Models.ViewModels
{
    public class McqQuestionWithOptionsViewModel
    {
        public int McqId { get; set; }
        public int McqTimer { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionDifficultyId { get; set; }
        public List<QuestionOptionViewModel> Options { get; set; }
    }
}
