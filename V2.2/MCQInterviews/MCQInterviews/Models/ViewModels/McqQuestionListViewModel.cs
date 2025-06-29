namespace MCQInterviews.Models.ViewModels
{
    public class McqQuestionListViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionDifficultyTypeId { get; set; }
        public string QuestionDifficultyTypeName { get; set; }
    }
}
