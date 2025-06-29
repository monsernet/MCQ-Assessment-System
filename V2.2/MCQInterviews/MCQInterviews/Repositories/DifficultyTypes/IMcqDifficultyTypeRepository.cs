using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.DifficultyTypes
{
    public interface IMcqDifficultyTypeRepository
    {
        Task<IEnumerable<McqDifficultyType>> GetMcqDifficultyTypesAsync();
        Task<McqDifficultyType?> GetMcqDifficultyTypeByIdAsync(int id);
        Task<McqDifficultyType> AddMcqDiffTypeAsync(McqDifficultyType mcqDiffType);
        Task<McqDifficultyType?> UpdateMcqDiffTypeAsync(McqDifficultyType mcqDiffType);
        Task<McqDifficultyType?> DeleteMcqDiffTypeAsync(McqDifficultyType mcqDiffType);
        Task<int> CountMcqDifficultyTypesAsync();

    }
}
