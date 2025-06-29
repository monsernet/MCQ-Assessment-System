using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.DifficultyTypes
{
    public class QuestionDifficultyTypeRepository : IQuestionDifficultyTypeRepository
    {
        private readonly ApplicaationDbContext applicationDbContext;

        public QuestionDifficultyTypeRepository(ApplicaationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<QuestionDifficultyType> AddQuestionDiffTypeAsync(QuestionDifficultyType questionDiffType)
        {
            await applicationDbContext.QuestionDifficultyTypes.AddAsync(questionDiffType);
            await applicationDbContext.SaveChangesAsync();
            return questionDiffType;
        }

        public async Task<QuestionDifficultyType?> DeleteQuestionDiffTypeAsync(QuestionDifficultyType questionDiffType)
        {
            applicationDbContext.QuestionDifficultyTypes.Remove(questionDiffType);
            await applicationDbContext.SaveChangesAsync();
            return questionDiffType;
        }

        public async Task<QuestionDifficultyType?> GetQuestionDifficultyTypeByIdAsync(int id)
        {
            var diffType = await applicationDbContext.QuestionDifficultyTypes.FirstOrDefaultAsync(x => x.Id == id);
            return diffType;
        }

        public async Task<IEnumerable<QuestionDifficultyType>> GetQuestionDifficultyTypesAsync()
        {
            var diffTypes = await applicationDbContext.QuestionDifficultyTypes.ToListAsync();
            return diffTypes;
        }

        public async Task<QuestionDifficultyType?> UpdateQuestionDiffTypeAsync(QuestionDifficultyType questionDiffType)
        {
            var questionDiffTypeToUpdate = await applicationDbContext.QuestionDifficultyTypes.FindAsync(questionDiffType.Id);
            if (questionDiffTypeToUpdate != null)
            {
                questionDiffTypeToUpdate.TypeText = questionDiffType.TypeText;
                questionDiffTypeToUpdate.PointValue = questionDiffType.PointValue;
                await applicationDbContext.SaveChangesAsync();
                return questionDiffTypeToUpdate;
            }
            else
            {
                return null;
            }
        }
    }
}
