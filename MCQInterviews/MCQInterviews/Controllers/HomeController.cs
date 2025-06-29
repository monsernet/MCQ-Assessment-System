using AuthSystem.Areas.Identity.Data;
using AuthSystem.Models;
using MCQInterviews.Repositories.JobLevels;
using MCQInterviews.Repositories.JobTitles;
using MCQInterviews.Repositories.MCQs;
using MCQInterviews.Repositories.Themes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AuthSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IThemeRepository themeRepository;
        private readonly IMCQRepository mCQRepository;
        private readonly IJobTitleRepository jobTitleRepository;
        private readonly IJobLevelRepository jobLevelRepository;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            IThemeRepository themeRepository,
            IMCQRepository mCQRepository,
            IJobTitleRepository jobTitleRepository,
            IJobLevelRepository jobLevelRepository
            )
        {
            _userManager = userManager;
            this.themeRepository = themeRepository;
            this.mCQRepository = mCQRepository;
            this.jobTitleRepository = jobTitleRepository;
            this.jobLevelRepository = jobLevelRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["UserID"] = _userManager.GetUserId(this.User);
            var themes = await themeRepository.GetThemesAsync();

            // Create a dictionary to store the MCQ counts for each theme
            var mcqCounts = new Dictionary<int, int>();

            // Iterate through each theme and get the MCQ count
            foreach (var theme in themes)
            {
                var mcqCount = await mCQRepository.GetMCQCountPerThemeAsync(theme.Id);
                mcqCounts.Add(theme.Id, mcqCount);
            }

            // Add the MCQ counts dictionary to ViewData
            ViewData["MCQCounts"] = mcqCounts;

            return View(themes);
        }

        public async Task<IActionResult> JobTitleMCQs(int themeId)
        {

            ViewData["UserID"] = _userManager.GetUserId(this.User);
            var jobTitles = await jobTitleRepository.GetJobTitlesByThemeAsync(themeId);

            // Create a dictionary to store the MCQ counts for each job category
            var mcqCounts = new Dictionary<int, int>();

            // Iterate through each job title and get the MCQ count
            foreach (var jobTitle in jobTitles)
            {
                var mcqCount = await mCQRepository.GetMCQCountPerJobTitleAsync(jobTitle.Id);
                mcqCounts.Add(jobTitle.Id, mcqCount);
            }

            // Add the MCQ counts dictionary to ViewData
            ViewData["MCQCounts"] = mcqCounts;

            return View(jobTitles);
        }

        public async Task<IActionResult> JobLevelMCQs(int jobTitle)
        {
            var jobLevels = await jobLevelRepository.GetJobLevelsAsync();
            ViewData["JobTitleId"] = jobTitle;
            
            var mcqCounts = new Dictionary<int, int>();
            foreach (var jobLevel in jobLevels)
            {
                var mcqCount = await mCQRepository.GetMCQCountPerJobLevelAsync(jobTitle, jobLevel.Id);
                mcqCounts.Add(jobLevel.Id, mcqCount);
            }

            ViewData["MCQCounts"] = mcqCounts;

            return View(jobLevels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}