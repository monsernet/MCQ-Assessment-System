using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.MCQTestResults
{
    public interface IMCQTestResultRepository
    {

        Task AddMCQTestResultAsync(MCQTestResult mcqTestResult);
        Task<List<MCQTestResult>> GetTestResultsByUserIdAsync(string userId);
        Task<int> GetTimesTakenAsync(string userId, int mcqId);
        Task<int> GetPassedTestsCountAsync();
        Task<int> GetPassedTestsCountByThemeAsync(int themeId);
        Task<IEnumerable<MCQTestResult>> GetTopScoresAsync(int count);
    }
}
