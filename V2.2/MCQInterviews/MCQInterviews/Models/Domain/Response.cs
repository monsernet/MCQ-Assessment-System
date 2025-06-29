namespace MCQInterviews.Models.Domain
{
    public class Response
    {
        public int Id { get; set; }

        // Foreign key to the question being answered
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        // For MCQ-type responses (select an option)
        public int? SelectedOptionId { get; set; }  // Nullable for non-MCQ types
        public QuestionOption SelectedOption { get; set; }

        // For text-based responses
        public string PlainTextResponse { get; set; }

        // For audio responses
        public string AudioUrl { get; set; }
    }
}
