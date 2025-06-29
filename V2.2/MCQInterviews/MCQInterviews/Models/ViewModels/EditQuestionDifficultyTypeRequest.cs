namespace MCQInterviews.Models.ViewModels
{
    public class EditQuestionDifficultyTypeRequest
    {
        public int Id { get; set; }
        public string TypeText { get; set; }
        public int PointValue { get; set; }
    }
}
