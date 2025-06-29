using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCQInterviews.Models.ViewModels
{
    public class EditQuestionRequest
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int ThemeId { get; set; }
        public int JobTitleId { get; set; }
        public int JobLevelId { get; set; }
        public int DifficultyTypeId { get; set; }
        public string ThemeName { get; set; }
        public List<SelectListItem> Themes { get; set; }
        public List<SelectListItem> JobTitles { get; set; }
        public List<SelectListItem> JobLevels { get; set; }
        public List<SelectListItem> DifficultyTypes { get; set; }
    }
}
