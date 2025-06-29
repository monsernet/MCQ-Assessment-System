namespace MCQInterviews.Models.ViewModels
{
    public class EditDifficultyAllocationInputModel
    {
        public int McqDifficultyTypeId { get; set; }
        public Dictionary<int, int> Percentages { get; set; }
    }
}
