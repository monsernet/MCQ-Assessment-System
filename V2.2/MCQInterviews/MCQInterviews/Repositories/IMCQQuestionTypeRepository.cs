using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories
{
    public interface IMCQQuestionTypeRepository
    {
        Task<Question> AddMCQQuestionAsync(Question question, List<QuestionOption> options);
        Task<IEnumerable<QuestionOption>> GetOptionsByQuestionIdAsync(int questionId);
    }
}
