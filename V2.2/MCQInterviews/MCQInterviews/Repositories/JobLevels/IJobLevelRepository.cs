using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.JobLevels
{
    public interface IJobLevelRepository
    {
        Task<IEnumerable<JobLevel>> GetJobLevelsAsync();
        Task<JobLevel?> GetJobLevelByIdAsync(int id);
        Task<JobLevel> AddJobLevelAsync(JobLevel jobLevel);
        Task<JobLevel?> UpdateJobLevelAsync(JobLevel jobLevel);
        Task<JobLevel?> DeleteJobLevelAsync(JobLevel jobLevel);
        Task<int> GetJobLevelsCountAsync();
    }
}
