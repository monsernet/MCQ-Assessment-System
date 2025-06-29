using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.DifficultyTypes;
using MCQInterviews.Repositories.JobLevels;
using MCQInterviews.Repositories.JobTitles;
using MCQInterviews.Repositories.MCQs;
using MCQInterviews.Repositories.MCQTestResults;
using MCQInterviews.Repositories.Questions;
using MCQInterviews.Repositories.Themes;
using MCQInterviews.Repositories.Users;

namespace MCQInterviews.Repositories.Admin
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly IThemeRepository themeRepository;
        private readonly IJobTitleRepository jobTitleRepository;
        private readonly IJobLevelRepository jobLevelRepository;
        private readonly IMCQRepository mCQRepository;
        private readonly IQuestionRepository questionRepository;
        private readonly IMCQTestResultRepository mCQTestResultRepository;
        private readonly IMcqDifficultyTypeRepository mcqDifficultyTypeRepository;
        private readonly IUserRepository userRepository;

        public AdminDashboardRepository(
            IThemeRepository themeRepository,
            IJobTitleRepository jobTitleRepository,
            IJobLevelRepository jobLevelRepository,
            IMCQRepository mCQRepository,
            IQuestionRepository questionRepository,
            IMCQTestResultRepository mCQTestResultRepository,
            IMcqDifficultyTypeRepository mcqDifficultyTypeRepository,
            IUserRepository userRepository
            )
        {
            this.themeRepository = themeRepository;
            this.jobTitleRepository = jobTitleRepository;
            this.jobLevelRepository = jobLevelRepository;
            this.mCQRepository = mCQRepository;
            this.questionRepository = questionRepository;
            this.mCQTestResultRepository = mCQTestResultRepository;
            this.mcqDifficultyTypeRepository = mcqDifficultyTypeRepository;
            this.userRepository = userRepository;
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardDataAsync()
        {
            var totalJobCategories = await themeRepository.GetThemesCountAsync();
            var totalJobTitles = await jobTitleRepository.GetJobTitlesCountAsync();
            var totalJobLevels = await jobLevelRepository.GetJobLevelsCountAsync();
            var totalMCQTests = await mCQRepository.GetMCQCountAsync();
            var totalQuestions = await questionRepository.GetQuestionsCountAsync();
            var totalTestPassed = await mCQTestResultRepository.GetPassedTestsCountAsync();
            var totalDiffTypes = await mcqDifficultyTypeRepository.CountMcqDifficultyTypesAsync();
            var totalUsers = await userRepository.GetNonAdminUserCountAsync();

            // Create an instance of AdminDashboardViewModel with the fetched data
            var adminDashboardData = new AdminDashboardViewModel
            {
                TotalJobCategories = totalJobCategories,
                TotalJobTitles = totalJobTitles,
                TotalJobLevels = totalJobLevels,
                TotalMCQTests = totalMCQTests,
                TotalQuestions = totalQuestions,
                TotalTestPassed = totalTestPassed,
                TotalDiffTypes = totalDiffTypes,
                TotalUsers = totalUsers

            };

            return adminDashboardData;
        }


    }
}
