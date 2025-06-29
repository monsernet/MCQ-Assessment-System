namespace MCQInterviews.Models.Domain
{
    public class McqQuestion
    {
        public int Id { get; set; }
        public int McqId { get; set; }
        public int QuestionId { get; set; }

        // Navigation property to the Question entity
        public Question Question { get; set; }
    }
}
