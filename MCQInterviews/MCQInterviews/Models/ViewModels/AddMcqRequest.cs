using Microsoft.AspNetCore.Mvc.Rendering;
using static MCQInterviews.Enums.Enums;

namespace MCQInterviews.Models.ViewModels
{
    public class AddMcqRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int NbQuestions { get; set; }
        public MCQType MCQType { get; set; }

        //to display the MCQ Types
        public List<SelectListItem> MCQTypeOptions { get; set; }

        public int ThemeId { get; set; }
        public int JobTitleId { get; set; }
        public int JobLevelId { get; set; }
        public int DifficultyTypeId { get; set; }

        // The name of the selected job title for display purposes
        public string JobTitleName { get; set; }

        // The name of the selected job level for display purposes
        public string JobLevelName { get; set; }
    }
}
