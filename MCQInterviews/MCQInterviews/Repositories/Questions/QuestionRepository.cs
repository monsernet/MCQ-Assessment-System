using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MCQInterviews.Repositories.Questions
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicaationDbContext applicaationDbContext;
        private IDbContextTransaction _transaction;

        public QuestionRepository(ApplicaationDbContext applicaationDbContext)
        {
            this.applicaationDbContext = applicaationDbContext;
        }

        public async Task<Question> AddJobTitleQuestion(int jobTitleId, Question question)
        {
            
            question.JobTitleId = jobTitleId;

            await applicaationDbContext.Questions.AddAsync(question);
            await applicaationDbContext.SaveChangesAsync();

            return question;
        }

        public async Task<Question> AddQuestionAsync(Question question)
        {
           
            await applicaationDbContext.Questions.AddAsync(question);
            await applicaationDbContext.SaveChangesAsync();
            return question;
        }

        public async Task<Question?> DeleteQuestionAsync(Question question)
        {
            var questionToDelete = await applicaationDbContext.Questions.FindAsync(question.Id);
            if (questionToDelete != null)
            {
                applicaationDbContext.Questions.Remove(questionToDelete);
                await applicaationDbContext.SaveChangesAsync();
                return questionToDelete;
            }
            else
            {
                return null;
            }
        }

        public async Task<QuestionOption> AddQuestionOptionAsync(QuestionOption questionOption)
        {
            await applicaationDbContext.QuestionOptions.AddAsync(questionOption); // Add the option to the database
            await applicaationDbContext.SaveChangesAsync(); // Save the changes
            return questionOption;
        }

        public async Task<Question?> GetQuestionByIdAsync(int id)
        {
            var searchedQuestion = await applicaationDbContext.Questions.FirstOrDefaultAsync(x => x.Id == id);
            return searchedQuestion;
        }

        public async Task<IEnumerable<Question>> GetQuestionsAsync()
        {
            var questions = await applicaationDbContext.Questions.ToListAsync();
            return questions;
        }

        public async Task<Question?> UpdateQuestionAsync(Question question)
        {
            var questionToUpdate = await applicaationDbContext.Questions.FindAsync(question.Id);
            if (questionToUpdate != null)
            {
                questionToUpdate.Text = question.Text;
                questionToUpdate.JobTitleId = question.JobTitleId;
                questionToUpdate.QuestionDifficultyTypeId = question.QuestionDifficultyTypeId;
                questionToUpdate.QuestionTypeId = question.QuestionTypeId;
                questionToUpdate.MediaUrl = question.MediaUrl;
                questionToUpdate.AudioUrl = question.AudioUrl;

                await applicaationDbContext.SaveChangesAsync();
                return questionToUpdate;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<Question>> GetExistingQuestions(int mcqId, int jobTitleId, int jobLevelId)
        {
            var query = applicaationDbContext.Questions.AsQueryable();

            // Filter by job title
            query = query.Where(p => p.JobTitleId == jobTitleId);

            // Retrieve existing question IDs for the MCQ (if applicable)
            var existingMcqQuestionIds = await GetMCQQuestionIdsAsync(mcqId);

            // Exclude questions present in the MCQ
            query = query.Where(q => !existingMcqQuestionIds.Contains(q.Id));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<int>> GetMCQQuestionIdsAsync(int mcqId)
        {
            
            var questionIds = await applicaationDbContext.McqQuestions
                                                 .Where(mq => mq.McqId == mcqId)
                                                 .Select(mq => mq.QuestionId)
                                                 .ToListAsync();

            return questionIds;
        }

        public async Task<int> GetQuestionsCountAsync()
        {
            return await applicaationDbContext.Questions.CountAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await applicaationDbContext.Database.BeginTransactionAsync();
            return _transaction;
        }
    }
}
