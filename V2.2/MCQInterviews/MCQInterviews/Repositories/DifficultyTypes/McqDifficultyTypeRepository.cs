using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.DifficultyTypes
{
    public class McqDifficultyTypeRepository : IMcqDifficultyTypeRepository
    {
        private readonly ApplicaationDbContext applicaationDbContext;

        public McqDifficultyTypeRepository(ApplicaationDbContext applicaationDbContext)
        {
            this.applicaationDbContext = applicaationDbContext;
        }
        public async Task<McqDifficultyType?> GetMcqDifficultyTypeByIdAsync(int id)
        {
            var diffType = await applicaationDbContext.McqDifficultyTypes.FirstOrDefaultAsync(x => x.Id == id);
            return diffType;
        }

        public async Task<IEnumerable<McqDifficultyType>> GetMcqDifficultyTypesAsync()
        {
            var diffTypes = await applicaationDbContext.McqDifficultyTypes.ToListAsync();
            return diffTypes;
        }

        public async Task<McqDifficultyType> AddMcqDiffTypeAsync(McqDifficultyType mcqDiffType)
        {
            await applicaationDbContext.McqDifficultyTypes.AddAsync(mcqDiffType);
            await applicaationDbContext.SaveChangesAsync();
            return mcqDiffType;
        }

        public async Task<McqDifficultyType?> DeleteMcqDiffTypeAsync(McqDifficultyType mcqDiffType)
        {
            var searchedMcqDiffType = await applicaationDbContext.McqDifficultyTypes.FindAsync(mcqDiffType.Id);
            if (searchedMcqDiffType != null)
            {
                applicaationDbContext.McqDifficultyTypes.Remove(mcqDiffType);
                await applicaationDbContext.SaveChangesAsync();
                return mcqDiffType;
            }
            else
            {
                return null;
            }
        }

        public async Task<McqDifficultyType?> UpdateMcqDiffTypeAsync(McqDifficultyType mcqDiffType)
        {
            var mcqDiffTypeToUpdate = await applicaationDbContext.McqDifficultyTypes.FindAsync(mcqDiffType.Id);
            if (mcqDiffTypeToUpdate != null)
            {
                mcqDiffTypeToUpdate.TypeText = mcqDiffType.TypeText;
                await applicaationDbContext.SaveChangesAsync();
                return mcqDiffTypeToUpdate;
            }
            else
            {
                return null;
            }
        }
        public async Task<int> CountMcqDifficultyTypesAsync()
        {
            var count = await applicaationDbContext.McqDifficultyTypes
                .CountAsync();

            return count;
        }
    }
}
