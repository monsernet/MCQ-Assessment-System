using Microsoft.AspNetCore.Mvc.Rendering;
using static MCQInterviews.Enums.Enums;

namespace MCQInterviews.Models.ViewModels
{
    public class EditMcqRequest
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
        // The list of Job categories for the dropdown
        public List<SelectListItem> Themes { get; set; }
        public int JobTitleId { get; set; }
        // The list of Job Titles for the dropdown
        public List<SelectListItem> JobTitles { get; set; }
        public int JobLevelId { get; set; }
        // The list of Job Levels for the dropdown
        public List<SelectListItem> JobLevels { get; set; }
        public int DifficultyTypeId { get; set; }
        // The list of Difficulty Types for the dropdown
        public List<SelectListItem> DifficultyTypes { get; set; }


    }
}
