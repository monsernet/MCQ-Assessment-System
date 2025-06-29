namespace MCQInterviews.Models.ViewModels
{
    public class OptionTestViewModel
    {
        public int OptionId { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }
    }
}
