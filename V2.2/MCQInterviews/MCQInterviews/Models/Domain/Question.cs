namespace MCQInterviews.Models.Domain
{
    public class Question
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }

        public int QuestionDifficultyTypeId { get; set; }
        public QuestionDifficultyType QuestionDifficultyType { get; set; }
        public int QuestionTypeId { get; set; } 
        public QuestionType QuestionType { get; set; } 

        public int ResponseTypeId { get; set; } 
        public ResponseType ResponseType { get; set; }

        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();

        // For media-type questions (video or image)
        public string? MediaUrl { get; set; }
        // For audio-type questions
        public string? AudioUrl { get; set; }
        public string? AudioResponseType { get; set; }
        public string? VideoResponseType { get; set; }
    }

}
