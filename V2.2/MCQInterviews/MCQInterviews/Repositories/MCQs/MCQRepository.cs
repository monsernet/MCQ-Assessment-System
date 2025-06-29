using MCQInterviews.Data;
using MCQInterviews.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Repositories.MCQs
{
    public class MCQRepository : IMCQRepository
    {
        private readonly ApplicaationDbContext _applicaationDbContext;

        public MCQRepository(ApplicaationDbContext applicaationDbContext)
        {
            _applicaationDbContext = applicaationDbContext;
        }
        public async Task<MCQ> AddMCQAsync(MCQ mcq)
        {
            await _applicaationDbContext.MCQs.AddAsync(mcq);
            await _applicaationDbContext.SaveChangesAsync();
            return mcq;
        }

        public async Task<MCQ?> DeleteMCQAsync(MCQ mcq)
        {
            var searchedMcq = await _applicaationDbContext.MCQs.FindAsync(mcq.Id);
            if (searchedMcq != null)
            {
                _applicaationDbContext.MCQs.Remove(mcq);
                await _applicaationDbContext.SaveChangesAsync();
                return mcq;
            }
            else
            {
                return null;
            }
        }

        public async Task<MCQ?> GetMCQByIdAsync(int id)
        {
            var mcq = await _applicaationDbContext.MCQs.FirstOrDefaultAsync(x => x.Id == id);
            return mcq;
        }

        public async Task<IEnumerable<MCQ>> GetMCQsAsync()
        {
            var mcqs = await _applicaationDbContext.MCQs.ToListAsync();
            return mcqs;
        }

        public async Task<IEnumerable<Question>> MCQQuestions(int jobTitleId, int jobLevelId)
        {
            var query = _applicaationDbContext.Questions.AsQueryable();

            query = query.Where(p => p.JobTitleId == jobTitleId);
            return await query.ToListAsync();
        }

        public async Task<MCQ?> UpdateMCQAsync(MCQ mcq)
        {
            var mcqToUpdate = await _applicaationDbContext.MCQs.FindAsync(mcq.Id);
            if (mcqToUpdate != null)
            {
                mcqToUpdate.Name = mcq.Name;
                mcqToUpdate.Description = mcq.Description;
                mcqToUpdate.Duration = mcq.Duration;
                mcqToUpdate.NbQuestions = mcq.NbQuestions;
                mcqToUpdate.MCQType = mcq.MCQType;
                mcqToUpdate.JobTitleId = mcq.JobTitleId;
                mcqToUpdate.JobLevel = mcq.JobLevel;
                await _applicaationDbContext.SaveChangesAsync();
                return mcqToUpdate;
            }
            else
            {
                return null;
            }

        }
        public async Task<int> GetMCQCountPerThemeAsync(int themeId)
        {
            var totalMCQCount = await _applicaationDbContext.Themes
                                .Where(theme => theme.Id == themeId)
                                .SelectMany(theme => theme.JobTitles)
                                .SelectMany(jobTitle => _applicaationDbContext.MCQs
                                .Where(mcq => mcq.JobTitleId == jobTitle.Id)
                                .Where(mcq => _applicaationDbContext.McqQuestions
                                .Count(mq => mq.McqId == mcq.Id) == mcq.NbQuestions))
                                .CountAsync();

            return totalMCQCount;

            
        }

        public async Task<int> GetMCQCountPerJobTitleAsync(int jobTitleId)
        {
            // Get the count of MCQs for a specific job title
            var mcqCount = await _applicaationDbContext.MCQs
                .Where(mcq => mcq.JobTitleId == jobTitleId)
                .Where(mcq => _applicaationDbContext.McqQuestions
                    .Count(mq => mq.McqId == mcq.Id) == mcq.NbQuestions)
                .CountAsync();

            return mcqCount;
        }

        public async Task<int> GetMCQCountPerJobLevelAsync(int jobTitleId, int jobLevelId)
        {
            // Get the count of MCQs for a specific job title and job level
            var mcqCount = await _applicaationDbContext.MCQs
                .Where(mcq => mcq.JobTitleId == jobTitleId && mcq.JobLevel == jobLevelId)
                .Where(mcq => _applicaationDbContext.McqQuestions
                    .Count(mq => mq.McqId == mcq.Id) == mcq.NbQuestions)
                .CountAsync();

            return mcqCount;
        }

        public async Task<int> GetMCQCountAsync()
        {
            return await _applicaationDbContext.MCQs.CountAsync();
        }

        public async Task<int> GetMCQsCountByThemeIdAsync(int themeId)
        {
            //ThemeId does not exist in MCQs table, we will extract the jobTitles related to the Theme
            return await _applicaationDbContext.MCQs
                .Where(mcq => mcq.JobTitle.ThemeId == themeId)
                .CountAsync();
        }

        public async Task<IEnumerable<MCQ>> GetMCQsByCategoryAsync(int categoryId)
        {
            var jobTitles = await _applicaationDbContext.JobTitles
                .Where(j => j.ThemeId == categoryId)
                .Select(j => j.Id)
                .ToListAsync();

            return await _applicaationDbContext.MCQs
                .Where(m => jobTitles.Contains(m.JobTitleId))
                .ToListAsync();
        }

        public async Task<IEnumerable<MCQ>> GetMCQsByCategoryAndJobTitleAsync(int categoryId, int jobTitleId)
        {
            return await _applicaationDbContext.MCQs
                .Where(m => m.JobTitleId == jobTitleId && m.JobTitle.ThemeId == categoryId)
                .ToListAsync();
        }

        public async Task<List<MCQ>> GetValidMCQsByJobTitleAndLevelAsync(int jobTitleId, int jobLevelId)
        {
            var validMcqs = await _applicaationDbContext.MCQs
            .Where(mcq => mcq.JobTitleId == jobTitleId && mcq.JobLevel == jobLevelId)
            .Where(mcq => _applicaationDbContext.McqQuestions.Count(mq => mq.McqId == mcq.Id) == mcq.NbQuestions)
            .ToListAsync();

            return validMcqs;
        }
    }
}
