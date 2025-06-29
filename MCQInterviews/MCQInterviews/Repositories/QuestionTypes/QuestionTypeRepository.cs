using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.QuestionTypes
{
    public class QuestionTypeRepository : IQuestionTypeRepository
    {
        private readonly ApplicaationDbContext _context;

        public QuestionTypeRepository(ApplicaationDbContext context)
        {
            _context = context;
        }

        public async Task<List<QuestionType>> GetQuestionTypesAsync()
        {
            return await _context.QuestionTypes.ToListAsync();
        }

        public async Task<QuestionType> GetQuestionTypeByIdAsync(int id)
        {
            return await _context.QuestionTypes.FindAsync(id);
        }

        public async Task<QuestionType> AddQuestionTypeAsync(QuestionType questionType)
        {
            _context.QuestionTypes.Add(questionType);
            await _context.SaveChangesAsync();
            return questionType;
        }

        public async Task<bool> UpdateQuestionTypeAsync(QuestionType questionType)
        {
            _context.QuestionTypes.Update(questionType);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteQuestionTypeAsync(int id)
        {
            var questionType = await _context.QuestionTypes.FindAsync(id);
            if (questionType != null)
            {
                _context.QuestionTypes.Remove(questionType);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
