using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.Options
{
    public interface IOptionRepository
    {
        Task<IEnumerable<QuestionOption>> GetOptionsAsync(int questionId);
        Task<QuestionOption?> GetOptionByIdAsync(int id);
        Task<QuestionOption> AddOptionAsync(int questionId, QuestionOption option);
        Task<QuestionOption?> UpdateOptionAsync(QuestionOption option);
        Task<QuestionOption?> DeleteOptionAsync(QuestionOption option);
        Task<IEnumerable<QuestionOption>> GetCorrectOptionByQuestionId(int questionId);
        Task<string> GetOptionTextByOptionId(int optionId);
        Task<int> OptionCountByQuestion(int questionId);

        //Reset the options of a question 
        Task ResetStatusForQuestionAsync(int questionId);
    }
}
