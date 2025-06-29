using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MCQInterviews.Models.ViewModels
{
    public class EditJobTitleRequest
    {
        public int Id { get; set; }

        // The name of the job title
        [Required(ErrorMessage = "Please enter the Job Title name.")]
        public string Name { get; set; }

        public int ThemeId { get; set; }

        // The list of Job Categories for the dropdown
        public List<SelectListItem> Themes { get; set; }
    }
}
