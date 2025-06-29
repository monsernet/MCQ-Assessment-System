using MCQInterviews.Models.Domain;

namespace MCQInterviews.Models.ViewModels
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int JobTitleId { get; set; }
        public int JobLevelId { get; set; }
        public int DifficultyTypeId { get; set; }
        public string ThemeName { get; set; }
        public string JobTitleName { get; set; }
        public string JobLevelName { get; set; }
        public string DifficultyTypeName { get; set; }
        public int SelectedJobTitleId { get; set; }
        public List<JobTitle> JobTitles { get; set; }

        public int SelectedJobLevelId { get; set; }
        public List<JobLevel> JobLevels { get; set; }

    }
}
