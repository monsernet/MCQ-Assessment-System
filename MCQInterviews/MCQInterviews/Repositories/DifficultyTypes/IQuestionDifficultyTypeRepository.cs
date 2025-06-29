using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.DifficultyTypes
{
    public interface IQuestionDifficultyTypeRepository
    {

        Task<IEnumerable<QuestionDifficultyType>> GetQuestionDifficultyTypesAsync();
        Task<QuestionDifficultyType?> GetQuestionDifficultyTypeByIdAsync(int id);
        Task<QuestionDifficultyType> AddQuestionDiffTypeAsync(QuestionDifficultyType questionDiffType);
        Task<QuestionDifficultyType?> UpdateQuestionDiffTypeAsync(QuestionDifficultyType questionDiffType);
        Task<QuestionDifficultyType?> DeleteQuestionDiffTypeAsync(QuestionDifficultyType questionDiffType);
    }
}
