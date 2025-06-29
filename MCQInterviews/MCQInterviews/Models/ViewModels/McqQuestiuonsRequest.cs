using MCQInterviews.Models.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using static MCQInterviews.Enums.Enums;

namespace MCQInterviews.Models.ViewModels
{
    public class McqQuestiuonsRequest
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
        // Job Categories for the dropdown
        public List<SelectListItem> Themes { get; set; }
        public int JobTitleId { get; set; }
        // Job Titles for the dropdown
        public List<SelectListItem> JobTitles { get; set; }
        public int JobLevelId { get; set; }
        // Job Levels for the dropdown
        public ICollection<Question> questions { get; set; }

        public List<SelectListItem> Questions { get; set; }
    }
}
