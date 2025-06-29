using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using MCQInterviews.Repositories.DifficultyTypes;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories
{
    public class MCQQuestionTypeRepository : IMCQQuestionTypeRepository
    {
        private readonly ApplicaationDbContext _context;

        public MCQQuestionTypeRepository(ApplicaationDbContext context)
        {
            _context = context;
        }

        // Adds MCQ Question and Question Options
        public async Task<Question> AddMCQQuestionAsync(Question question, List<QuestionOption> options)
        {
            if (question.QuestionTypeId != 1)
            {
                throw new InvalidOperationException("Only MCQ questions can have options.");
            }

            // Add MCQ Question to the Questions table
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();

            // Now add the options to the QuestionOptions table
            foreach (var option in options)
            {
                option.QuestionId = question.Id; // Associate each option with the question
                await _context.QuestionOptions.AddAsync(option);
            }

            await _context.SaveChangesAsync();

            return question;
        }

        // Retrieves options for a given question
        public async Task<IEnumerable<QuestionOption>> GetOptionsByQuestionIdAsync(int questionId)
        {
            return await _context.QuestionOptions
                                 .Where(o => o.QuestionId == questionId)
                                 .ToListAsync();
        }
    }
}
