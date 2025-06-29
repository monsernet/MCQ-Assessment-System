namespace MCQInterviews.Models.ViewModels
{
    public class McqDifficultyTypeGroupViewModel
    {
        public int McqDifficultyTypeId { get; set; }
        public string McqDifficultyTypeName { get; set; }
        public List<DifficultyAllocationViewModel> Allocations { get; set; } = new List<DifficultyAllocationViewModel>();
    }
}
