using System.ComponentModel.DataAnnotations;

namespace MCQInterviews.Models.ViewModels
{
    public class VoiceSampleUploadViewModel
    {
        [Required]
        [Display(Name = "Voice Sample")]
        public IFormFile VoiceSample { get; set; }
    }
}
