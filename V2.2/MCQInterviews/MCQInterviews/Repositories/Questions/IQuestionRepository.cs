using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore.Storage;

namespace MCQInterviews.Repositories.Questions
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetQuestionsAsync();
        Task<Question?> GetQuestionByIdAsync(int id);
        Task<Question> AddQuestionAsync(Question question);
        Task<Question?> UpdateQuestionAsync(Question question);
        Task<Question?> DeleteQuestionAsync(Question question);
        Task<QuestionOption> AddQuestionOptionAsync(QuestionOption questionOption);
        Task<Question> AddJobTitleQuestion(int jobTitleId, Question question);
        Task<IEnumerable<Question>> GetExistingQuestions(int mcqId, int jobTitleId, int jobLevelId);
        Task<IEnumerable<int>> GetMCQQuestionIdsAsync(int mcqId);
        Task<int> GetQuestionsCountAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
