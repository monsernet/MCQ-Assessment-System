namespace MCQInterviews.Models.ViewModels
{
    public class EditDifficultyAllocationViewModel
    {
        public int McqDifficultyTypeId { get; set; }
        public string McqDifficultyTypeName { get; set; }
        public List<QuestionDifficultyTypeAllocation> Allocations { get; set; }
    }
}
