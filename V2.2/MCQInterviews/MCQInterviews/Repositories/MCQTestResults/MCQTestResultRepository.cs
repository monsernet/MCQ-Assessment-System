using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.MCQTestResults
{
    public class MCQTestResultRepository : IMCQTestResultRepository
    {
        private readonly ApplicaationDbContext applicaationDbContext;

        public MCQTestResultRepository(ApplicaationDbContext applicaationDbContext)
        {
            this.applicaationDbContext = applicaationDbContext;
        }
        public async Task AddMCQTestResultAsync(MCQTestResult mcqTestResult)
        {
            await applicaationDbContext.MCQTestResults.AddAsync(mcqTestResult);
            await applicaationDbContext.SaveChangesAsync();
        }

        public async Task<int> GetPassedTestsCountAsync()
        {
            return await applicaationDbContext.MCQTestResults.CountAsync();
        }

        //Get the tests the user has passed
        public async Task<List<MCQTestResult>> GetTestResultsByUserIdAsync(string userId)
        {
            return await applicaationDbContext.MCQTestResults
                .Where(tr => tr.UserId == userId)
                .ToListAsync();
        }

        //count the number of times the user has taken the MCQ test
        public async Task<int> GetTimesTakenAsync(string userId, int mcqId)
        {
            return await applicaationDbContext.MCQTestResults
                .CountAsync(tr => tr.UserId == userId && tr.MCQId == mcqId);
        }

        public async Task<int> GetPassedTestsCountByThemeAsync(int themeId)
        {
            // Get the jobTitleIds associated with the specified themeId
            var jobTitleIds = await applicaationDbContext.JobTitles
                .Where(jt => jt.ThemeId == themeId)
                .Select(jt => jt.Id)
                .ToListAsync();

            // Get the mcqIds associated with the jobTitleIds
            var mcqIds = await applicaationDbContext.MCQs
                .Where(mcq => jobTitleIds.Contains(mcq.JobTitleId))
                .Select(mcq => mcq.Id)
                .ToListAsync();

            // Get the count of passed tests for the obtained mcqIds
            var passedTestsCount = await applicaationDbContext.MCQTestResults
                .Where(result => mcqIds.Contains(result.MCQId))
                .CountAsync();

            return passedTestsCount;
        }

        public async Task<IEnumerable<MCQTestResult>> GetTopScoresAsync(int count)
        {
            return await applicaationDbContext.MCQTestResults
               .Include(r => r.User)
               .Include(r => r.MCQ)
               .OrderByDescending(r => r.Score)
               .Take(count)
               .ToListAsync();
        }
    }
}
