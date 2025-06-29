using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.Options
{
    public class OptionRepository : IOptionRepository
    {
        private readonly ApplicaationDbContext applicaationDbContext;

        public OptionRepository(ApplicaationDbContext applicaationDbContext)
        {
            this.applicaationDbContext = applicaationDbContext;
        }

        public async Task<QuestionOption> AddOptionAsync(int questionId, QuestionOption option)
        {
            option.QuestionId = questionId;
            await applicaationDbContext.QuestionOptions.AddAsync(option);
            await applicaationDbContext.SaveChangesAsync();
            return option;
        }

        public async Task<QuestionOption?> DeleteOptionAsync(QuestionOption option)
        {
            var optionToDelete = await applicaationDbContext.QuestionOptions.FindAsync(option.Id);
            if (optionToDelete != null)
            {
                applicaationDbContext.QuestionOptions.Remove(optionToDelete);
                await applicaationDbContext.SaveChangesAsync();
                return optionToDelete;
            }
            else
            {
                return null;
            }
        }

        public async Task<QuestionOption?> GetOptionByIdAsync(int id)
        {
            var option = await applicaationDbContext.QuestionOptions.FirstOrDefaultAsync(x => x.Id == id);
            return option;

        }

        public async Task<IEnumerable<QuestionOption>> GetOptionsAsync(int questionId)
        {
            var query = applicaationDbContext.QuestionOptions.AsQueryable();

            query = query.Where(p => p.QuestionId == questionId);
            return await query.ToListAsync();
        }

        public async Task<QuestionOption?> UpdateOptionAsync(QuestionOption option)
        {
            var optionToUpdate = await applicaationDbContext.QuestionOptions.FirstOrDefaultAsync(o => o.Id == option.Id);
            if (optionToUpdate != null)
            {
                optionToUpdate.Text = option.Text;
                await applicaationDbContext.SaveChangesAsync();
                return optionToUpdate;
            }
            else
            {
                return null;
            }
        }

        public async Task ResetStatusForQuestionAsync(int questionId)
        {
            var options = await applicaationDbContext.QuestionOptions.Where(o => o.QuestionId == questionId).ToListAsync();

            foreach (var option in options)
            {
                option.IsCorrect = false;
            }

            await applicaationDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<QuestionOption>> GetCorrectOptionByQuestionId(int questionId)
        {
            var correctOption = await applicaationDbContext.QuestionOptions
                .Where(option => option.QuestionId == questionId && option.IsCorrect)
                .ToListAsync();
            if (correctOption != null)
            {
                return correctOption;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> GetOptionTextByOptionId(int optionId)
        {
            var option = await applicaationDbContext.QuestionOptions.FindAsync(optionId);
            if (option == null)
            {
                throw new InvalidOperationException($"No option found with ID {optionId}");
            }
            return option.Text;
        }

        public async Task<int> OptionCountByQuestion(int questionId)
        {
            var count = await applicaationDbContext.QuestionOptions
                 .Where(x => x.QuestionId == questionId)
                 .CountAsync();

            return count;
        }
    }
}
