namespace MCQInterviews.Models.ViewModels
{
    public class QuestionResultViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int SelectedOptionId { get; set; }
        public bool IsCorrect { get; set; }
        public string SelectedOptionText { get; set; }
        public string CorrectOptionText { get; set; }
    }
}
