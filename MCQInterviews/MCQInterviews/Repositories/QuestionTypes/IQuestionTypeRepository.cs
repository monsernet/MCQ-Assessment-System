using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.QuestionTypes
{
    public interface IQuestionTypeRepository
    {
        Task<List<QuestionType>> GetQuestionTypesAsync();
        Task<QuestionType> GetQuestionTypeByIdAsync(int id);
        Task<QuestionType> AddQuestionTypeAsync(QuestionType questionType);
        Task<bool> UpdateQuestionTypeAsync(QuestionType questionType);
        Task<bool> DeleteQuestionTypeAsync(int id);
    }
}
