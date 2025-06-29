using static MCQInterviews.Enums.Enums;

namespace MCQInterviews.Models.ViewModels
{
    public class McqViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int NbQuestions { get; set; }
        public MCQType MCQType { get; set; }
        public int ThemeId { get; set; }
        public int JobTitleId { get; set; }
        public int JobLevelId { get; set; }
        public int DifficultyTypeId { get; set; }
        public string ThemeName { get; set; }
        public string JobTitleName { get; set; }
        public string JobLevelName { get; set; }
        public string DifficultyTypeName { get; set; }
        public int AddedQuestions { get; set; }
    }
}
