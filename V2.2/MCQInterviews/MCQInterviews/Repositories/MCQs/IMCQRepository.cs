using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.MCQs
{
    public interface IMCQRepository
    {
        Task<IEnumerable<MCQ>> GetMCQsAsync();
        Task<MCQ?> GetMCQByIdAsync(int id);
        Task<MCQ> AddMCQAsync(MCQ mcq);
        Task<MCQ?> UpdateMCQAsync(MCQ mcq);
        Task<MCQ?> DeleteMCQAsync(MCQ mcq);
        Task<IEnumerable<Question>> MCQQuestions(int jobTitleId, int jobLevelId);
        Task<int> GetMCQsCountByThemeIdAsync(int themeId);

        Task<int> GetMCQCountPerThemeAsync(int themeId);
        Task<int> GetMCQCountPerJobTitleAsync(int jobTitleId);
        Task<int> GetMCQCountPerJobLevelAsync(int jobTitleId, int jobLevelId);
        Task<int> GetMCQCountAsync();
        Task<IEnumerable<MCQ>> GetMCQsByCategoryAsync(int categoryId);
        Task<IEnumerable<MCQ>> GetMCQsByCategoryAndJobTitleAsync(int categoryId, int jobTitleId);
        Task<List<MCQ>> GetValidMCQsByJobTitleAndLevelAsync(int jobTitleId, int jobLevelId);





    }
}
