using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;

namespace MCQInterviews.Repositories.McqQuestions
{
    public interface IMcqQuestionRepository
    {
        Task<IEnumerable<McqQuestion>> GetAllMcqQuestionsAsync();
        Task<McqQuestion?> GetMcqQuestionByIdAsync(int mcqId, int questionId);
        Task<McqQuestion> AddMcqQuestionAsync(McqQuestion mcqQuestion);
        Task<McqQuestion?> UpdateMcqQuestionAsync(McqQuestion mcqQuestion);
        Task<McqQuestion?> DeleteMcqQuestionAsync(int mcqId, int questionId);

        Task<List<McqQuestionViewModel>> GetMcqQuestionsAsync(int mcqId);
        Task<McqQuestion?> AddMcqQuestionWithQuestionIdAsync(int mcqId, int questionId);
        Task<int> CountMcqQuestionsAsync(int mcqId);

        Task<List<McqQuestionWithOptionsViewModel>> GetMcqQuestionsWithOptionsAsync(int mcqId);
        Dictionary<string, int> CountQuestionsByDifficultyTypeAsync(int mcqId);
        Task<int> GetDifficultyPointsAsync(int questionId);

    }
}
