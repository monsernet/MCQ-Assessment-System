namespace MCQInterviews.Models.ViewModels
{
    public class McqQuestionViewModel
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public int McqId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionDifficultyTypeText { get; set; }
        public int nbOptions { get; set; }
        public string nbOptionsCheck { get; set; }

    }
}
