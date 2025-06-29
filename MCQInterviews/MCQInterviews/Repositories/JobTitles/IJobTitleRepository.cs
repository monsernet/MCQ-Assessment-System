using MCQInterviews.Models.Domain;

namespace MCQInterviews.Repositories.JobTitles
{
    public interface IJobTitleRepository
    {
        Task<IEnumerable<JobTitle>> GetJobTitlesAsync();
        Task<JobTitle?> GetJobTitleByIdAsync(int id);
        Task<JobTitle> AddJobTitleAsync(JobTitle jobTitle);
        Task<JobTitle?> UpdateJobTitleAsync(JobTitle jobTitle);
        Task<JobTitle?> DeleteJobTitleAsync(JobTitle jobTitle);
        Task<int> GetJobTitlesCountAsync();
        Task<int> GetTotalMCQsById(int jobTitleId);
        Task<int> GetTotalPassedMCQTestsById(int jobTitleId);

        // retreive list of job titles by theme
        Task<IEnumerable<JobTitle>> GetJobTitlesByThemeAsync(int themeId);
        Task<int> GetJobTitlesCountByThemeIdAsync(int themeId);

    }
}
