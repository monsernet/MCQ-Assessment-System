using MCQInterviews.Models.Domain;

namespace MCQInterviews.Models.ViewModels
{
    public class AddQuestionRequest
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int ThemeId { get; set; }
        public int JobTitleId { get; set; }
        public int JobLevelId { get; set; }
        public int DifficultyTypeId { get; set; }
        public string ThemeName { get; set; }
        public int QuestionTypeId { get; set; } // New property for selecting question type
        public int ResponseTypeId { get; set; } // New property for selecting response type
        public List<QuestionOption>? Options { get; set; } // Only required for MCQ
        public string? PhotoUrl { get; set; } // Optional for media questions
        public string? VideoUrl { get; set; } // Optional for media questions
        public string? AudioUrl { get; set; } // Optional for audio questions
        public string? AudioResponseType { get; set; }
        public string? VideoResponseType { get; set; }

    }
}
