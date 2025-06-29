using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCQInterviews.Models.ViewModels
{
    public class EditDifficultyAllocationRequest
    {
        public int Id { get; set; }
        public double Percentage { get; set; }
        public int McqDifficultyTypeId { get; set; }
        public int QuestionDifficultyTypeId { get; set; }

        // The list of Mcq Difficulty Types for the dropdown
        public List<SelectListItem> McqDifficultyTypes { get; set; }

        // The list of Question Difficulty Types for the dropdown
        public List<SelectListItem> QuestionDifficultyTypes { get; set; }
    }
}
