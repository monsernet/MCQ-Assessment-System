using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.JobTitles
{
    public class JobTitleRepository : IJobTitleRepository
    {
        private readonly ApplicaationDbContext applicaationDbContext;

        public JobTitleRepository(ApplicaationDbContext applicaationDbContext)
        {
            this.applicaationDbContext = applicaationDbContext;
        }

        public async Task<JobTitle> AddJobTitleAsync(JobTitle jobTitle)
        {
            await applicaationDbContext.JobTitles.AddAsync(jobTitle);
            await applicaationDbContext.SaveChangesAsync();
            return jobTitle;
        }

        public async Task<JobTitle?> DeleteJobTitleAsync(JobTitle jobTitle)
        {
            var SearchedJobTitle = await applicaationDbContext.JobTitles.FindAsync(jobTitle.Id);
            if (SearchedJobTitle != null)
            {
                applicaationDbContext.JobTitles.Remove(jobTitle);
                await applicaationDbContext.SaveChangesAsync();
                return SearchedJobTitle;
            }
            else
            {
                return null;
            }
        }

        public async Task<JobTitle?> GetJobTitleByIdAsync(int id)
        {
            var jobTitle = await applicaationDbContext.JobTitles.FirstOrDefaultAsync(x => x.Id == id);
            return jobTitle;
        }

        public async Task<IEnumerable<JobTitle>> GetJobTitlesAsync()
        {
            var jobTitles = await applicaationDbContext.JobTitles.ToListAsync();
            return jobTitles;
        }

        public async Task<JobTitle?> UpdateJobTitleAsync(JobTitle jobTitle)
        {
            var jobTitleToUpdate = await applicaationDbContext.JobTitles.FindAsync(jobTitle.Id);
            if (jobTitleToUpdate != null)
            {
                jobTitleToUpdate.Name = jobTitle.Name;
                jobTitleToUpdate.ThemeId = jobTitle.ThemeId;
                await applicaationDbContext.SaveChangesAsync();
                return jobTitleToUpdate;
            }
            else
            {
                return null;
            }
        }
        public async Task<IEnumerable<JobTitle>> GetJobTitlesByThemeAsync(int themeId)
        {
            return await applicaationDbContext.JobTitles
                .Where(jt => jt.ThemeId == themeId)
                .ToListAsync();
        }

        public async Task<int> GetJobTitlesCountByThemeIdAsync(int themeId)
        {
            return await applicaationDbContext.JobTitles
                .Where(jt => jt.ThemeId == themeId)
                .CountAsync();
        }

        public async Task<int> GetJobTitlesCountAsync()
        {
            return await applicaationDbContext.JobTitles.CountAsync();
        }

        public async Task<int> GetTotalMCQsById(int jobTitleId)
        {
            return await applicaationDbContext.MCQs
                .Where(jt => jt.JobTitleId == jobTitleId)
                .CountAsync();
        }

        public async Task<int> GetTotalPassedMCQTestsById(int jobTitleId)
        {
            return await applicaationDbContext.MCQTestResults
                .Where(jt => jt.MCQ.JobTitleId == jobTitleId)
                .CountAsync();
        }
    }
}
