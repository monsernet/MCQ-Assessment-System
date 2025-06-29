namespace MCQInterviews.Models.ViewModels
{
    public class UpdateDifficultyAllocationsViewModel
    {
        public int McqDifficultyTypeId { get; set; }
        public List<AllocationViewModel> Allocations { get; set; }
    }
}
