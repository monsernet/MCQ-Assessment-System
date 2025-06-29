namespace MCQInterviews.Models.Domain
{
    public class QuestionDifficultyType
    {
        public int Id { get; set; }
        public string TypeText { get; set; }

        //To manage points for users based on the Question Difficulty Type
        public int PointValue { get; set; }

        // Navigation property
        public ICollection<Question> Questions { get; set; }

        // New navigation property for DifficultyAllocations
        public ICollection<DifficultyAllocation> DifficultyAllocations { get; set; }
    }
}
