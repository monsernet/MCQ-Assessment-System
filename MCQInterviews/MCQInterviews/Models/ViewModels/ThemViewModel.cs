using MCQInterviews.Models.Domain;

namespace MCQInterviews.Models.ViewModels
{
    public class ThemeViewModel
    {
        public IEnumerable<Theme> Themes { get; set; }
        public List<int> MCQCounts { get; set; }
    }
}
