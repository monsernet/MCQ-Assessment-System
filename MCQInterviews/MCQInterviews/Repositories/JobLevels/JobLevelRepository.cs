using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.JobLevels
{
    public class JobLevelRepository : IJobLevelRepository
    {
        private readonly ApplicaationDbContext _applicaationDbContext;

        public JobLevelRepository(ApplicaationDbContext applicaationDbContext)
        {
            _applicaationDbContext = applicaationDbContext;
        }
        public async Task<JobLevel> AddJobLevelAsync(JobLevel jobLevel)
        {
            await _applicaationDbContext.JobLevels.AddAsync(jobLevel);
            await _applicaationDbContext.SaveChangesAsync();
            return jobLevel;

        }

        public async Task<JobLevel?> DeleteJobLevelAsync(JobLevel jobLevel)
        {

            _applicaationDbContext.JobLevels.Remove(jobLevel);
            await _applicaationDbContext.SaveChangesAsync();
            return jobLevel;

        }

        public async Task<JobLevel?> GetJobLevelByIdAsync(int id)
        {
            var jobLevel = await _applicaationDbContext.JobLevels.FirstOrDefaultAsync(x => x.Id == id);
            return jobLevel;
        }

        public async Task<IEnumerable<JobLevel>> GetJobLevelsAsync()
        {
            var jobLevels = await _applicaationDbContext.JobLevels.ToListAsync();
            return jobLevels;
        }

        public async Task<int> GetJobLevelsCountAsync()
        {
            return await _applicaationDbContext.JobLevels.CountAsync();
        }

        public async Task<JobLevel?> UpdateJobLevelAsync(JobLevel jobLevel)
        {
            var jobLevelToUpdate = await _applicaationDbContext.JobLevels.FindAsync(jobLevel.Id);
            if (jobLevelToUpdate != null)
            {
                jobLevelToUpdate.Name = jobLevel.Name;
                await _applicaationDbContext.SaveChangesAsync();
                return jobLevelToUpdate;
            }
            else
            {
                return null;
            }
        }
    }
}
