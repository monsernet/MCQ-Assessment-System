namespace MCQInterviews.Models.ViewModels

{
    public class McqTestViewModel
    {

        public int QuestionId { get; set; }
        public string Text { get; set; }
        public string QuestionDifficultyTypeText { get; set; }
        public List<OptionTestViewModel> Options { get; set; }
    }
}
