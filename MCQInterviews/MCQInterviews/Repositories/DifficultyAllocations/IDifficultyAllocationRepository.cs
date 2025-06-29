using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.DifficultyAllocations
{
    public interface IDifficultyAllocationRepository
    {
        Task<IEnumerable<DifficultyAllocation>> GetAllAsync();
        Task<DifficultyAllocation?> GetByIdAsync(int id);
        Task<DifficultyAllocation> AddAsync(DifficultyAllocation difficultyAllocation);
        Task<DifficultyAllocation?> UpdateAsync(DifficultyAllocation difficultyAllocation);
        Task<DifficultyAllocation?> DeleteAsync(DifficultyAllocation difficultyAllocation);
        Task<List<DifficultyAllocation>> GetMcqDifficultyAllocations(int mcqDiffTypeId);
        Task<IEnumerable<DifficultyAllocation>> GetAllocationsByMcqDifficultyTypeIdAsync(int mcqDifficultyTypeId);
        Task<DifficultyAllocation> GetByMcqAndQuestionTypeIdsAsync(int mcqDifficultyTypeId, int questionDifficultyTypeId);
    }
}
