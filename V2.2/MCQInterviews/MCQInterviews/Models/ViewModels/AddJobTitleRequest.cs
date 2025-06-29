using System.ComponentModel.DataAnnotations;


namespace MCQInterviews.Models.ViewModels
{
    public class AddJobTitleRequest
    {
        public int Id { get; set; }

        // The name of the job title
        [Required(ErrorMessage = "Please enter the Job Title name.")]
        public string Name { get; set; }

        // The ID of the selected theme (category) for the job title
        [Required(ErrorMessage = "Please select a category.")]
        public int ThemeId { get; set; }

        // The name of the selected job title for display purposes
        public string JobTitleName { get; set; }
    }
}
