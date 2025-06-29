using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCQInterviews.Models.ViewModels
{
    public class JobTitleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ThemeId { get; set; }
        public string ThemeName { get; set; }

        // properties for filtering
        public int SelectedCategoryId { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
