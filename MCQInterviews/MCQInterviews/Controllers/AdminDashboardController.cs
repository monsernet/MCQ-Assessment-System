using AuthSystem.Areas.Identity.Data;
using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.Admin;
using MCQInterviews.Repositories.DifficultyAllocations;
using MCQInterviews.Repositories.DifficultyTypes;
using MCQInterviews.Repositories.JobLevels;
using MCQInterviews.Repositories.JobTitles;
using MCQInterviews.Repositories.MCQs;
using MCQInterviews.Repositories.MCQTestResults;
using MCQInterviews.Repositories.Themes;
using MCQInterviews.Repositories.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;




namespace MCQInterviews.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAdminDashboardRepository adminDashboardRepository;
        private readonly IUserRepository userRepository;
        private readonly IThemeRepository themeRepository;
        private readonly IJobTitleRepository jobTitleRepository;
        private readonly IMCQRepository mCQRepository;
        private readonly IMCQTestResultRepository mCQTestResultRepository;
        private readonly IJobLevelRepository jobLevelRepository;
        private readonly IDifficultyAllocationRepository difficultyAllocationRepository;
        private readonly IMcqDifficultyTypeRepository mcqDifficultyTypeRepository;
        private readonly IQuestionDifficultyTypeRepository questionDifficultyTypeRepository;

        public AdminDashboardController(
            UserManager<ApplicationUser> userManager,
            IAdminDashboardRepository adminDashboardRepository,
            IUserRepository userRepository,
            IThemeRepository themeRepository,
            IJobTitleRepository jobTitleRepository,
            IMCQRepository mCQRepository,
            IMCQTestResultRepository mCQTestResultRepository,
            IJobLevelRepository jobLevelRepository,
            IDifficultyAllocationRepository difficultyAllocationRepository,
            IMcqDifficultyTypeRepository mcqDifficultyTypeRepository,
            IQuestionDifficultyTypeRepository questionDifficultyTypeRepository
            )
        {
            this.userManager = userManager;
            this.adminDashboardRepository = adminDashboardRepository;
            this.userRepository = userRepository;
            this.themeRepository = themeRepository;
            this.jobTitleRepository = jobTitleRepository;
            this.mCQRepository = mCQRepository;
            this.mCQTestResultRepository = mCQTestResultRepository;
            this.jobLevelRepository = jobLevelRepository;
            this.difficultyAllocationRepository = difficultyAllocationRepository;
            this.mcqDifficultyTypeRepository = mcqDifficultyTypeRepository;
            this.questionDifficultyTypeRepository = questionDifficultyTypeRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Fetch overall admin dashboard data
            var adminDashboardData = await adminDashboardRepository.GetAdminDashboardDataAsync();

            // ******* Fetch data for User Management ********
            var users = await userRepository.GetNonAdminUsers();

            var userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                var numberOfLogins = await userRepository.UserLogins(user.Id);

                userViewModels.Add(new UserViewModel
                {
                    User = user,
                    NumberOfLogins = numberOfLogins
                });
            }
            // ******* Fetch data for Job Category Management ********
            var jobCategories = await themeRepository.GetThemesAsync();

            // Map Theme entities to ThemeManagementViewModel
            var jobCategoriesViewModels = new List<ThemeManagementViewModel>();
            foreach (var theme in jobCategories)
            {
                var jobTitlesCount = await jobTitleRepository.GetJobTitlesCountByThemeIdAsync(theme.Id);
                var mcqCount = await mCQRepository.GetMCQsCountByThemeIdAsync(theme.Id);
                var testsPassedCount = await mCQTestResultRepository.GetPassedTestsCountByThemeAsync(theme.Id);

                jobCategoriesViewModels.Add(new ThemeManagementViewModel
                {
                    Id = theme.Id,
                    Name = theme.Name,
                    TotalJobTitles = jobTitlesCount,
                    TotalMCQTests = mcqCount,
                    TotalTestsPassed = testsPassedCount,

                });
            }

            // ******* Fetch data for Job Title Management ********
            var jobTitles = await jobTitleRepository.GetJobTitlesAsync();
            var jobTitleViewModels = jobTitles
                  .Select(jobTitle =>
                  {
                      if (jobTitle == null)
                      {
                          return null; // Skip null jobTitles
                      }

                      var jobTitleView = jobTitleRepository.GetJobTitleByIdAsync(jobTitle.Id).Result;
                      var jobTitleName = jobTitleView?.Name ?? "";
                      var categoryId = jobTitleView?.ThemeId;
                      var categoryName = categoryId != null ? themeRepository.GetThemeByIdAsync(categoryId.Value).Result?.Name ?? "" : "";
                      var nbMCQTests = jobTitleRepository.GetTotalMCQsById(jobTitle.Id).Result;
                      var nbPassedTests = jobTitleRepository.GetTotalPassedMCQTestsById(jobTitle.Id).Result;

                      return new JobTitleManagementViewModel
                      {
                          Id = jobTitle.Id,
                          JobTitleName = jobTitleName,
                          JobCategoryName = categoryName,
                          TotalMCQTests = nbMCQTests,
                          TotalPassedTests = nbPassedTests,
                      };
                  })
                  .ToList();

            // ******* Fetch data for Job Level Management ********
            var jobLevels = await jobLevelRepository.GetJobLevelsAsync();
            var jobLevelViewModels = new List<JobLevelManagementViewModel>();
            foreach (var jobLevel in jobLevels)
            {
                jobLevelViewModels.Add(new JobLevelManagementViewModel
                {
                    Id = jobLevel.Id,
                    Name = jobLevel.Name
                });
            }

            // ******* Fetch data for Difficulty Allocation Management ********
            var allocations = await difficultyAllocationRepository.GetAllAsync();
            var allocationViewModels = await MapAllocationsToViewModelsAsync(allocations);

            // ******* Fetch data for MCQ Test Management ********
            var mcqTests = await mCQRepository.GetMCQsAsync();
            var mcqViewModels = await MapMcqsToViewModelsAsync(mcqTests);
            var mcqTestViewModels = new List<McqViewModel>();
            foreach (var test in mcqViewModels)
            {
                mcqTestViewModels.Add(new McqViewModel
                {
                    Id = test.Id,
                    Name = test.Name,
                    ThemeName = test.ThemeName,
                    JobTitleName = test.JobTitleName,
                    JobLevelId = test.JobLevelId,
                    JobTitleId = test.JobTitleId,
                    JobLevelName = test.JobLevelName,
                    NbQuestions = test.NbQuestions,
                    Duration = test.Duration,
                });
            }

            // ******* Fetch data for TOP 10 Users scores  ********

            var topScores = await mCQTestResultRepository.GetTopScoresAsync(10);

            var userScores = topScores
                .GroupBy(result => result.UserId)
                .Select(group => new
                {
                    UserId = group.Key,
                    TotalTestsPassed = group.Count(),
                    AverageScore = group.Average(result => result.Score),
                    BestScore = group.Max(result => result.Score)
                });

            // Find the top 10 users based on the best score
            var topUsers = userScores
                .OrderByDescending(userScore => userScore.BestScore)
                .Take(10);


            var topUsersDetails = new List<TopUserDetailsViewModel>();
            foreach (var topUser in topUsers)
            {
                var user = await userRepository.GetUserById(topUser.UserId);

                // Calculate the overall performance metric (composite of average score and assessments passed)
                double overallPerformance = CalculateOverallPerformance(topUser.AverageScore, topUser.TotalTestsPassed);

                // Find the assessment with the best score for this user
                var bestScoreResult = topScores.FirstOrDefault(result => result.UserId == topUser.UserId && result.Score == topUser.BestScore);

                if (bestScoreResult != null)
                {
                    var mcq = await mCQRepository.GetMCQByIdAsync(bestScoreResult.MCQId);
                    if (mcq != null)
                    {
                        var jobTitle = await jobTitleRepository.GetJobTitleByIdAsync(mcq.JobTitleId);
                        if (jobTitle != null)
                        {
                            var jobCategory = await themeRepository.GetThemeByIdAsync(jobTitle.ThemeId);

                            var userDetails = new TopUserDetailsViewModel
                            {
                                UserName = $"{user.FirstName} {user.LastName}",
                                Country = user.Country ?? "N/A",
                                TestsPassed = topUser.TotalTestsPassed,
                                AverageScore = topUser.AverageScore,
                                BestJobCategory = jobCategory?.Name ?? "N/A",
                                OverallPerformance = overallPerformance
                            };
                            topUsersDetails.Add(userDetails);
                        }
                    }
                }
            }

            // Sort users by overall performance
            topUsersDetails = topUsersDetails.OrderByDescending(u => u.OverallPerformance).ToList();


            // ******* Fetch All Data for Admin Dashboard View Model ********
            var adminDashboardViewModel = new AdminDashboardViewModel
            {
                TotalUsers = adminDashboardData.TotalUsers,
                TotalJobCategories = adminDashboardData.TotalJobCategories,
                TotalJobTitles = adminDashboardData.TotalJobTitles,
                TotalJobLevels = adminDashboardData.TotalJobLevels,
                TotalMCQTests = adminDashboardData.TotalMCQTests,
                TotalQuestions = adminDashboardData.TotalQuestions,
                TotalTestPassed = adminDashboardData.TotalTestPassed,
                TotalDiffTypes = adminDashboardData.TotalDiffTypes,
                Users = userViewModels,
                JobCategories = jobCategoriesViewModels,
                JobTitles = jobTitleViewModels!,
                JobLevels = jobLevelViewModels,
                McqTests = mcqTestViewModels,
                TopUsersDetails = topUsersDetails,
                DifficultyAllocations = allocationViewModels

            };


            return View(adminDashboardViewModel);
        }

        private async Task<List<McqViewModel>> MapMcqsToViewModelsAsync(IEnumerable<MCQ> mcqs)
        {
            var mcqViewModels = new List<McqViewModel>();

            foreach (var mcq in mcqs)
            {
                var jobTitleView = await jobTitleRepository.GetJobTitleByIdAsync(mcq.JobTitleId);
                var jobLevelView = await jobLevelRepository.GetJobLevelByIdAsync(mcq.JobLevel);
                if (jobTitleView != null)
                {
                    var themeId = jobTitleView.ThemeId;
                    var themeView = await themeRepository.GetThemeByIdAsync(themeId);
                    var themeName = themeView?.Name ?? "N/A";


                    var viewModel = new McqViewModel
                    {
                        Id = mcq.Id,
                        Name = mcq.Name,
                        Description = mcq.Description,
                        Duration = mcq.Duration,
                        NbQuestions = mcq.NbQuestions,
                        MCQType = mcq.MCQType,
                        ThemeId = themeId,
                        ThemeName = themeName,
                        JobTitleId = mcq.JobTitleId,
                        JobTitleName = jobTitleView.Name,
                        JobLevelId = mcq.JobLevel,
                        JobLevelName = jobLevelView?.Name ?? "N/A"
                    };
                    mcqViewModels.Add(viewModel);
                }

            }

            return mcqViewModels;
        }

        private async Task<List<DifficultyAllocationViewModel>> MapAllocationsToViewModelsAsync(IEnumerable<DifficultyAllocation> allocations)
        {
            var allocationViewModels = new List<DifficultyAllocationViewModel>();

            foreach (var allocation in allocations)
            {
                var mcqDiffType = await mcqDifficultyTypeRepository.GetMcqDifficultyTypeByIdAsync(allocation.McqDifficultyTypeId);
                var questionDiffType = await questionDifficultyTypeRepository.GetQuestionDifficultyTypeByIdAsync(allocation.QuestionDifficultyTypeId);
                var viewModel = new DifficultyAllocationViewModel
                {
                    Id = allocation.Id,
                    Percentage = allocation.Percentage,
                    McqDifficultyTypeId = allocation.McqDifficultyTypeId,
                    McqDifficultyTypeName = mcqDiffType?.TypeText ?? "N/A",
                    QuestionDifficultyTypeId = allocation.QuestionDifficultyTypeId,
                    QuestionDifficultyTypeName = questionDiffType?.TypeText ?? "N/A"

                };
                allocationViewModels.Add(viewModel);
            }

            return allocationViewModels;
        }


        private static double CalculateOverallPerformance(double averageScore, int testsPassed)
        {
            // Weights for average score and tests passed
            double averageScoreWeight = 0.8;
            double testsPassedWeight = 0.2;

            // Calculate the overall performance as a weighted sum
            double overallPerformance = (averageScoreWeight * averageScore) + (testsPassedWeight * testsPassed);

            return overallPerformance;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await userRepository.DeactivateUser(userId);
                    TempData["successMsg"] = "User successfully deactivated.";
                }
                else
                {
                    TempData["errorMsg"] = "Error occurred when trying to deactivate the user. Please try again.";
                }
            }
            else
            {
                TempData["errorMsg"] = "Error occurred when trying to deactivate the user. Please try again.";
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {

                    await userRepository.ActivateUser(userId);
                    TempData["successMsg"] = "User successfully activated.";
                }
                else
                {
                    TempData["errorMsg"] = "Error occurred when trying to activate the user. Please try again.";
                }
            }
            else
            {
                TempData["errorMsg"] = "Error occurred when trying to activate the user. Please try again.";
            }

            return RedirectToAction("Index");
        }

    }
}
