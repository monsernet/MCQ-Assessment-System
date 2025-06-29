using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.McqQuestions
{
    public class McqQuestionRepository : IMcqQuestionRepository
    {
        private readonly ApplicaationDbContext applicaationDbContext;
        public McqQuestionRepository(ApplicaationDbContext applicaationDbContext)
        {
            this.applicaationDbContext = applicaationDbContext;
        }

        public async Task<McqQuestion> AddMcqQuestionAsync(McqQuestion mcqQuestion)
        {
            await applicaationDbContext.McqQuestions.AddAsync(mcqQuestion);
            await applicaationDbContext.SaveChangesAsync();
            return mcqQuestion;
        }

        public async Task<McqQuestion?> DeleteMcqQuestionAsync(int mcqId, int questionId)
        {
            var searchedMcqQuestion = await applicaationDbContext.McqQuestions.FirstOrDefaultAsync(x => x.McqId == mcqId && x.QuestionId == questionId);
            if (searchedMcqQuestion != null)
            {
                applicaationDbContext.McqQuestions.Remove(searchedMcqQuestion);
                await applicaationDbContext.SaveChangesAsync();
            }

            return searchedMcqQuestion;
        }

        public async Task<McqQuestion?> GetMcqQuestionByIdAsync(int mcqId, int questionId)
        {
            var mcqQuestion = await applicaationDbContext.McqQuestions.FirstOrDefaultAsync(x => x.McqId == mcqId && x.QuestionId == questionId);
            return mcqQuestion;
        }

        public async Task<List<McqQuestionViewModel>> GetMcqQuestionsAsync(int mcqId)
        {

            var questions = await applicaationDbContext.McqQuestions
                .Where(p => p.McqId == mcqId)
                .Join(
                    applicaationDbContext.Questions,
                    mcqQuestion => mcqQuestion.QuestionId,
                    question => question.Id,
                    (mcqQuestion, question) => new McqQuestionViewModel
                    {
                        QuestionText = question.Text,
                        McqId = mcqQuestion.McqId,
                        QuestionId = mcqQuestion.QuestionId,
                        QuestionDifficultyTypeText = question.QuestionDifficultyType.TypeText
                    }
                )
                .ToListAsync();

            return questions;

        }

        public async Task<IEnumerable<McqQuestion>> GetAllMcqQuestionsAsync()
        {
            var mcqQuestions = await applicaationDbContext.McqQuestions.ToListAsync();
            return mcqQuestions;
        }

        public async Task<McqQuestion?> UpdateMcqQuestionAsync(McqQuestion mcqQuestion)
        {
            var mcqQuestionToUpdate = await applicaationDbContext.McqQuestions.FindAsync(mcqQuestion.Id);
            if (mcqQuestionToUpdate != null)
            {
                mcqQuestionToUpdate.QuestionId = mcqQuestion.QuestionId;
                mcqQuestionToUpdate.McqId = mcqQuestion.McqId;

                await applicaationDbContext.SaveChangesAsync();
                return mcqQuestionToUpdate;
            }
            else
            {
                return null;
            }
        }

        public async Task<McqQuestion?> AddMcqQuestionWithQuestionIdAsync(int mcqId, int questionId)
        {
            var mcqQuestion = new McqQuestion
            {
                McqId = mcqId,
                QuestionId = questionId
            };

            await applicaationDbContext.McqQuestions.AddAsync(mcqQuestion);
            await applicaationDbContext.SaveChangesAsync();

            return mcqQuestion;
        }

        public async Task<int> CountMcqQuestionsAsync(int mcqId)
        {
            var count = await applicaationDbContext.McqQuestions
                .Where(x => x.McqId == mcqId)
                .CountAsync();

            return count;
        }

        public async Task<List<McqQuestionWithOptionsViewModel>> GetMcqQuestionsWithOptionsAsync(int mcqId)
        {
            var mcqQuestionsWithOptions = await applicaationDbContext.McqQuestions
                .Where(p => p.McqId == mcqId)
                .Include(q => q.Question)
                .Select(mcqQuestion => new McqQuestionWithOptionsViewModel
                {
                    McqId = mcqQuestion.McqId,
                    QuestionId = mcqQuestion.QuestionId,
                    QuestionText = mcqQuestion.Question.Text,
                    QuestionDifficultyId = mcqQuestion.Question.QuestionDifficultyTypeId,
                   /* Options = mcqQuestion.Question.QuestionOptions
                        .Select(option => new QuestionOptionViewModel
                        {
                            OptionId = option.Id,
                            OptionText = option.Text
                        })
                        .ToList()*/
                })
                .ToListAsync();

            return mcqQuestionsWithOptions;
        }

        public Dictionary<string, int> CountQuestionsByDifficultyTypeAsync(int mcqId)
        {
            var questionCountsByDifficultyType = applicaationDbContext.McqQuestions
            .Where(mcqQuestion => mcqQuestion.McqId == mcqId)
            .Join(
                applicaationDbContext.Questions,
                mcqQuestion => mcqQuestion.QuestionId,
                question => question.Id,
                (mcqQuestion, question) => new
                {
                    QuestionDifficultyTypeId = question.QuestionDifficultyTypeId,
                }
            )
            .GroupBy(result => result.QuestionDifficultyTypeId)
            .ToDictionary(
                group => group.Key.ToString(),  
                group => group.Count()
            );

            return questionCountsByDifficultyType;
        }

        public async Task<int> GetDifficultyPointsAsync(int questionId)
        {
            var question = await applicaationDbContext.Questions
                .Where(q => q.Id == questionId)
                .Include(q => q.QuestionDifficultyType)
                .FirstOrDefaultAsync();

            return question?.QuestionDifficultyType?.PointValue ?? 0;
        }


    }
}
