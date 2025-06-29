namespace MCQInterviews.Models.ViewModels
{
    public class AddDifficultyAllocationRequest
    {
        public int Id { get; set; }
        public double Percentage { get; set; }
        public int McqDifficultyTypeId { get; set; }
        public int QuestionDifficultyTypeId { get; set; }
    }
}
