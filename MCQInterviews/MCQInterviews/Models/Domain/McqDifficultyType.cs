namespace MCQInterviews.Models.Domain
{
    public class McqDifficultyType
    {
        public int Id { get; set; }
        public string TypeText { get; set; }

        // Navigation property
        public ICollection<MCQ> MCQs { get; set; }

        // New navigation property for DifficultyAllocations
        public ICollection<DifficultyAllocation> DifficultyAllocations { get; set; }
    }
}
