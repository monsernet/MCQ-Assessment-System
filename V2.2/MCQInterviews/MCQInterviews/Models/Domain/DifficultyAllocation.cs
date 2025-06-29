namespace MCQInterviews.Models.Domain
{
    public class DifficultyAllocation
    {
        public int Id { get; set; }
        public double Percentage { get; set; }

        // Foreign keys
        public int McqDifficultyTypeId { get; set; }
        public int QuestionDifficultyTypeId { get; set; }

        // Navigation properties
        public McqDifficultyType McqDifficultyType { get; set; }
        public QuestionDifficultyType QuestionDifficultyType { get; set; }
    }
}
