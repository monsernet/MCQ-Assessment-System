using AuthSystem.Areas.Identity.Data;

using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.JobLevels;
using MCQInterviews.Repositories.JobTitles;
using MCQInterviews.Repositories.MCQs;
using MCQInterviews.Repositories.MCQTestResults;
using MCQInterviews.Repositories.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MCQInterviews.Controllers
{
    [Authorize]
    public class UserDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IMCQTestResultRepository _mCQTestResultRepository;
        private readonly IJobLevelRepository _jobLevelRepository;
        private readonly IJobTitleRepository _jobTitleRepository;
        private readonly IMCQRepository _mCQRepository;

        public UserDashboardController(
            UserManager<ApplicationUser> userManager,
            IUserRepository userRepository,
            IMCQTestResultRepository mCQTestResultRepository,
            IJobLevelRepository jobLevelRepository,
            IJobTitleRepository jobTitleRepository,
            IMCQRepository mCQRepository
            )
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _mCQTestResultRepository = mCQTestResultRepository;
            _jobLevelRepository = jobLevelRepository;
            _jobTitleRepository = jobTitleRepository;
            _mCQRepository = mCQRepository;
        }
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);

            // Retrieve user data and test results from the database
            var user = await _userManager.FindByIdAsync(userId);
            var testResults = await _mCQTestResultRepository.GetTestResultsByUserIdAsync(userId);

            // Calculate statistics
            var totalMCQs = testResults.Count();
            // Calculate the sum of scores from all passed tests
            var totalScore = testResults.Sum(tr => tr.Score);
            // Calculate the sum of points from all passed tests
            // (for duplicates tests , consider only the test with highest point score )
            var testResultsGroupedByTestId = testResults.GroupBy(tr => tr.MCQId); 

            var totalPoints = 0;
            foreach (var testGroup in testResultsGroupedByTestId)
            {
                // Find the maximum score for this test (if the user has taken it multiple times)
                var maxScoreForTest = testGroup.Max(tr => tr.Points);

                // If the user has retaken the test more than 1 time, consider only the highest point score
                totalPoints += maxScoreForTest;
            }
            // Calculate the progress percentage
            double progressPercentage = totalMCQs > 0
                ? ((double)totalScore / (totalMCQs * 100)) * 100
                : 0;

            // Create DashboardViewModel
            var dashboardViewModel = new DashboardViewModel
            {
                UserId = userId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                JoinDate = user.RegistrationDate,
                Country = user.Country,
                ProfileImage = user.ProfileImage,
                TotalMCQs = totalMCQs,
                TotalPoints = totalPoints,
                ProgressPercentage = progressPercentage,
                Achievements = GetAchievements(testResults),
                Feedback = GenerateFeedback(progressPercentage)
            };

            // Retrieve achieved tests
            var viewModelList = new List<AchievedTestViewModel>();

            foreach (var testResult in testResults)
            {
                var mcqId = testResult.MCQId;
                var mcq = await _mCQRepository.GetMCQByIdAsync(mcqId);
                var jobLevelId = mcq.JobLevel;
                var jobTitleId = mcq.JobTitleId;

                var jobLevelDetails = await _jobLevelRepository.GetJobLevelByIdAsync(jobLevelId);
                var jobLevelName = jobLevelDetails.Name;
                var jobTitleDetails = await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId);
                var jobTitleName = jobTitleDetails.Name;

                var viewModel = new AchievedTestViewModel
                {
                    MCQId = mcqId,
                    MCQName = testResult.MCQ.Name,
                    JobTitleName = jobTitleName,
                    JobLevelName = jobLevelName,
                    Score = testResult.Score,
                    DateTaken = testResult.DateTaken
                };

                viewModelList.Add(viewModel);
            }

            ViewBag.AchievedTests = viewModelList;

            return View(dashboardViewModel);
        }

        public async Task<IActionResult> UserDashboard(string userId)
        {

            // Retrieve user data and test results from the database
            var user = await _userManager.FindByIdAsync(userId);
            var testResults = await _mCQTestResultRepository.GetTestResultsByUserIdAsync(userId);

            // Calculate statistics
            var totalMCQs = testResults.Count();
            // Calculate the sum of scores from all passed tests
            var totalScore = testResults.Sum(tr => tr.Score);
            // Calculate the sum of points from all passed tests
            // (for duplicates tests , consider only the test with highest point score )
            var testResultsGroupedByTestId = testResults.GroupBy(tr => tr.MCQId); 

            var totalPoints = 0;

            foreach (var testGroup in testResultsGroupedByTestId)
            {
                // Find the maximum score for this test (if the user has taken it multiple times)
                var maxScoreForTest = testGroup.Max(tr => tr.Points);

                // If the user has retaken the test more than 1 time, consider only the highest points score
                totalPoints += maxScoreForTest;
            }
            // Calculate the progress percentage
            double progressPercentage = totalMCQs > 0
                ? ((double)totalScore / (totalMCQs * 100)) * 100
                : 0;

            // Create DashboardViewModel
            var dashboardViewModel = new DashboardViewModel
            {
                UserId = userId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                JoinDate = user.RegistrationDate,
                Country = user.Country,
                ProfileImage = user.ProfileImage,
                TotalMCQs = totalMCQs,
                TotalPoints = totalPoints,
                ProgressPercentage = progressPercentage,
                Achievements = GetAchievements(testResults),
                Feedback = GenerateFeedback(progressPercentage) // Implement GenerateFeedback method
            };

            // Retrieve achieved tests
            var viewModelList = new List<AchievedTestViewModel>();

            foreach (var testResult in testResults)
            {
                var mcqId = testResult.MCQId;
                var mcq = await _mCQRepository.GetMCQByIdAsync(mcqId);
                var jobLevelId = mcq.JobLevel;
                var jobTitleId = mcq.JobTitleId;

                var jobLevelDetails = await _jobLevelRepository.GetJobLevelByIdAsync(jobLevelId);
                var jobLevelName = jobLevelDetails.Name;
                var jobTitleDetails = await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId);
                var jobTitleName = "N/A";
                if (jobTitleDetails != null)
                {
                    jobTitleName = jobTitleDetails.Name;
                }

                var viewModel = new AchievedTestViewModel
                {
                    MCQId = mcqId,
                    MCQName = testResult.MCQ.Name,
                    JobTitleName = jobTitleName,
                    JobLevelName = jobLevelName,
                    Score = testResult.Score,
                    DateTaken = testResult.DateTaken
                };

                viewModelList.Add(viewModel);
            }

            ViewBag.AchievedTests = viewModelList;

            return View(dashboardViewModel);
        }


        //Get the Badges awarded by the user 
        private List<string> GetAchievements(List<MCQTestResult> testResults)
        {
            var achievements = new List<string>();

            // Badge 1: Apprentice Tester
            if (testResults.Any())
            {
                achievements.Add("Apprentice Tester");
            }

            // Badge 2: Consistent Achiever
            var consistentAchieverCount = testResults
             .Where(tr => tr.Score > 70)
             .Select(tr => tr.MCQId)
             .Distinct()
             .Count();

            if (consistentAchieverCount >= 5)
            {
                achievements.Add("Consistent Achiever");
            }

            // Badge 3: Elite Performer
            var eliteAchieverCount = testResults
            .Where(tr => tr.Score > 90)
            .Select(tr => tr.MCQId)
            .Distinct()
            .Count();

            if (eliteAchieverCount >= 5)
            {
                achievements.Add("Elite Performer");
            }

            // Badge 4: Master of Levels
            var distinctJobTitles = testResults
                .Where(tr => tr.MCQ != null && tr.MCQ.JobTitleId >= 0)
                .Select(tr => tr.MCQ.JobTitleId)
                .Distinct()
                .Count();
            if (distinctJobTitles >= 5)
            {
                achievements.Add("Master of Levels");
            }

            // Badge 5: Premium Tester
            if (testResults.Any(tr => tr.MCQ != null && Equals(tr.MCQ.MCQType, 1)))
            {
                achievements.Add("Premium Tester");
            }

            // Badge 6: Diversified Expert
            var distinctJobCategories = testResults
                .Select(tr => $"{tr.MCQ?.JobTitleId}-{tr.MCQ?.JobTitle?.ThemeId}")
                .Distinct()
                .Count();
            if (distinctJobCategories > 3)
            {
                achievements.Add("Diversified Expert");
            }

            // Badge 7: Decathlete
            if (testResults.Count(tr => tr.Score >= 70) >= 10)
            {
                achievements.Add("Decathlete");
            }

            // Badge 8: Score Achiever
            if (testResults.Count(tr => tr.Score >= 70) > 20)
            {
                achievements.Add("Score Achiever");
            }

            // Badge 9: Master Tester
            if (testResults.Count(tr => tr.Score >= 70) > 30)
            {
                achievements.Add("Master Tester");
            }


            var categoryMaestroCount = testResults
                .GroupBy(tr => tr.MCQ?.JobTitleId)
                .Select(group => new
                {
                    ThemeId = group.First()?.MCQ?.JobTitle?.ThemeId,
                    Count = group.Count()
                })
                .ToList();

            if (categoryMaestroCount.Count() >= 5)
            {
                achievements.Add("Category Maestro");
            }

            return achievements;
        }

        private string GenerateFeedback(double progressPercentage)
        {

            if (progressPercentage >= 90)
            {
                return "Congratulations ! You're excelling. Keep up the excellent work!";
            }
            else if (progressPercentage >= 70)
            {
                return "Great job! You're performing well. Keep practicing to improve further.";
            }
            else if (progressPercentage >= 50)
            {
                return "Good effort! You're making progress. Keep practicing to achieve better results.";
            }
            else
            {
                return "Keep going! With consistent practice, you can improve your performance.";
            }
        }

        //Get the Achieved test for the current user
        public async Task<IActionResult> AchievedTests()
        {
            var userId = _userManager.GetUserId(User);
            var testResults = await _mCQTestResultRepository.GetTestResultsByUserIdAsync(userId);

            var viewModelList = new List<AchievedTestViewModel>();

            foreach (var testResult in testResults)
            {
                var mcqId = testResult.MCQId;
                var mcq = await _mCQRepository.GetMCQByIdAsync(mcqId);
                var jobLevelId = mcq.JobLevel;
                var jobTitleId = mcq.JobTitleId;

                var jobLevelDetails = await _jobLevelRepository.GetJobLevelByIdAsync(jobLevelId);
                var jobLevelName = jobLevelDetails.Name;
                var jobTitleDetails = await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId);
                if (jobTitleDetails != null)
                {
                    var jobTitleName = jobTitleDetails.Name;

                    var viewModel = new AchievedTestViewModel
                    {
                        MCQId = mcqId,
                        MCQName = testResult.MCQ.Name,
                        JobTitleName = jobTitleName,
                        JobLevelName = jobLevelName,
                        Score = testResult.Score,
                        DateTaken = testResult.DateTaken
                    };

                    viewModelList.Add(viewModel);
                }

            }

            return View(viewModelList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userRepository.DeactivateUser(userId);
                return View(user);
            }
            else
            {
                return View(null);
            }
        }



    }
}
