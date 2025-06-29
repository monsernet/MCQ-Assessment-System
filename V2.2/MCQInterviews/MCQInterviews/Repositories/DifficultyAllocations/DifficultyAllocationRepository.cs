using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.DifficultyAllocations
{
    public class DifficultyAllocationRepository : IDifficultyAllocationRepository
    {
        private readonly ApplicaationDbContext applicaationDbContext;

        public DifficultyAllocationRepository(ApplicaationDbContext applicaationDbContext)
        {
            this.applicaationDbContext = applicaationDbContext;
        }
        public async Task<DifficultyAllocation> AddAsync(DifficultyAllocation difficultyAllocation)
        {
            await applicaationDbContext.DifficultyAllocations.AddAsync(difficultyAllocation);
            await applicaationDbContext.SaveChangesAsync();
            return difficultyAllocation;
        }

        public async Task<DifficultyAllocation?> DeleteAsync(DifficultyAllocation difficultyAllocation)
        {
            var searchedAllocation = await applicaationDbContext.DifficultyAllocations.FindAsync(difficultyAllocation.Id);
            if (searchedAllocation != null)
            {
                applicaationDbContext.DifficultyAllocations.Remove(difficultyAllocation);
                await applicaationDbContext.SaveChangesAsync();
                return difficultyAllocation;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<DifficultyAllocation>> GetAllAsync()
        {
            var allocations = await applicaationDbContext.DifficultyAllocations.ToListAsync();
            return allocations;
        }

        public async Task<DifficultyAllocation?> GetByIdAsync(int id)
        {
            var allocation = await applicaationDbContext.DifficultyAllocations.FirstOrDefaultAsync(x => x.Id == id);
            return allocation;
        }

        public async Task<DifficultyAllocation?> UpdateAsync(DifficultyAllocation difficultyAllocation)
        {

            var allocationToUpdate = await applicaationDbContext.DifficultyAllocations.FindAsync(difficultyAllocation.Id);
            if (allocationToUpdate != null)
            {
                allocationToUpdate.Percentage = difficultyAllocation.Percentage;
                allocationToUpdate.QuestionDifficultyTypeId = difficultyAllocation.QuestionDifficultyTypeId;
                allocationToUpdate.McqDifficultyTypeId = difficultyAllocation.McqDifficultyTypeId;
                await applicaationDbContext.SaveChangesAsync();
                return allocationToUpdate;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<DifficultyAllocation>> GetMcqDifficultyAllocations(int mcqDiffTypeId)
        {
            return await applicaationDbContext.DifficultyAllocations
                .Include(da => da.QuestionDifficultyType)
                .Where(da => da.McqDifficultyTypeId == mcqDiffTypeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DifficultyAllocation>> GetAllocationsByMcqDifficultyTypeIdAsync(int mcqDifficultyTypeId)
        {
            return await applicaationDbContext.DifficultyAllocations
                             .Where(da => da.McqDifficultyTypeId == mcqDifficultyTypeId)
                             .ToListAsync();
        }

        public async Task<DifficultyAllocation> GetByMcqAndQuestionTypeIdsAsync(int mcqDifficultyTypeId, int questionDifficultyTypeId)
        {
            var diffAllocations = await applicaationDbContext.DifficultyAllocations
             .FirstOrDefaultAsync(da => da.McqDifficultyTypeId == mcqDifficultyTypeId && da.QuestionDifficultyTypeId == questionDifficultyTypeId);
            //if (diffAllocations == null)
            //{
            //    throw new InvalidOperationException("Difficulty allocations not found.");
            //}
            return diffAllocations;
        }
    }
}
