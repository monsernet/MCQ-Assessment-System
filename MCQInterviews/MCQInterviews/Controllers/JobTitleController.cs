using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.JobTitles;
using MCQInterviews.Repositories.Themes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCQInterviews.Controllers
{
    [Authorize(Roles = "Admin, Editor")]
    public class JobTitleController : Controller
    {
        private readonly IJobTitleRepository jobTitleRepository;
        private readonly IThemeRepository themeRepository;

        public JobTitleController(IJobTitleRepository jobTitleRepository, IThemeRepository themeRepository)
        {
            this.jobTitleRepository = jobTitleRepository;
            this.themeRepository = themeRepository;
        }
        //**** Add new JobTitle
        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            //Get Job Categories from Repository
            var themes = await themeRepository.GetThemesAsync();

            var themeList = themes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            ViewBag.Themes = themeList;
            return View();
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddJobTitleRequest addJobTitleRequest)
        {
            var jobTitle = new JobTitle
            {
                Name = addJobTitleRequest.Name,
                ThemeId = addJobTitleRequest.ThemeId
            };
            await jobTitleRepository.AddJobTitleAsync(jobTitle);
            TempData["successMsg"] = "Job Title added successfully";
            return RedirectToAction("List");
        }

        //***** Get the details of a Job Title
        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Get the job title by id
            var jobTitle = await jobTitleRepository.GetJobTitleByIdAsync(id);
            if (jobTitle == null)
            {
                return NotFound();
            }

            // Get all Job Categories from the repository
            var themes = await themeRepository.GetThemesAsync();
            var themeList = themes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();

            // Find the selected Job Category name  based on the job title's ThemeId
            var selectedTheme = await themeRepository.GetThemeByIdAsync(jobTitle.ThemeId);

            // Create the ViewModel
            var jobTitleViewModel = new EditJobTitleRequest
            {
                Id = jobTitle.Id,
                Name = jobTitle.Name,
                ThemeId = jobTitle.ThemeId,
                Themes = themeList 
            };
            return View(jobTitleViewModel);
        }

        //**** Save the updates made on the Job Title 
        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditJobTitleRequest editJobTitleRequest)
        {

            var jobTitle = new JobTitle
            {
                Id = editJobTitleRequest.Id,
                Name = editJobTitleRequest.Name,
                ThemeId = editJobTitleRequest.ThemeId
            };
            var updatedJobTitle = await jobTitleRepository.UpdateJobTitleAsync(jobTitle);
            if (updatedJobTitle != null)
            {
                TempData["SuccessMsg"] = "Job Title updated successfully";
                return RedirectToAction("List");
            }
            else
            {
                TempData["ErrorMsg"] = "Error occurred. Job Title not updated";
                return View(editJobTitleRequest);
            }

        }

        //**** Get the list of Job Titles
        [HttpGet]
        public async Task<IActionResult> List(int? categoryId = null)
        {

            IEnumerable<JobTitle> jobTitles;

            if (categoryId.HasValue)
            {
                jobTitles = await jobTitleRepository.GetJobTitlesByThemeAsync(categoryId.Value);
            }
            else
            {
                jobTitles = await jobTitleRepository.GetJobTitlesAsync();
            }

            var jobTitleViewModels = await MapJobTitlesToViewModelsAsync(jobTitles);

            var categories = await themeRepository.GetThemesAsync();
            var categoryList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            // Ensure there is at least one job title view model to set the job category list and selected job category id
            if (jobTitleViewModels.Any())
            {
                foreach (var viewModel in jobTitleViewModels)
                {
                    viewModel.Categories = categoryList;
                    viewModel.SelectedCategoryId = categoryId ?? 0;
                }
            }
            else
            {
                // If there are no job titles, create a placeholder view model to hold the job category list and selected job category id
                jobTitleViewModels.Add(new JobTitleViewModel
                {
                    Categories = categoryList,
                    SelectedCategoryId = categoryId ?? 0
                });
            }

            return View(jobTitleViewModels);

        }

        private async Task<List<JobTitleViewModel>> MapJobTitlesToViewModelsAsync(IEnumerable<JobTitle> jobTitles)
        {
            var jobTitleViewModels = new List<JobTitleViewModel>();

            foreach (var jobTitle in jobTitles)
            {
                var theme = await themeRepository.GetThemeByIdAsync(jobTitle.ThemeId);
                var viewModel = new JobTitleViewModel
                {
                    Id = jobTitle.Id,
                    Name = jobTitle.Name,
                    ThemeId = jobTitle.ThemeId,
                    ThemeName = theme?.Name 
                };
                jobTitleViewModels.Add(viewModel);
            }

            return jobTitleViewModels;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobTitlesByCategory(int categoryId)
        {
            var jobTitles = await jobTitleRepository.GetJobTitlesByThemeAsync(categoryId);
            var jobTitleList = jobTitles.Select(j => new SelectListItem
            {
                Value = j.Id.ToString(),
                Text = j.Name
            }).ToList();

            return Json(jobTitleList);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var jobTitleToDelete = await jobTitleRepository.GetJobTitleByIdAsync(id);
            //If requested Job Title is found
            if (jobTitleToDelete != null)
            {
                var deletedJobTitle = await jobTitleRepository.DeleteJobTitleAsync(jobTitleToDelete);
                if (deletedJobTitle != null)
                {
                    // success message - Job Title deleted
                    TempData["successMsg"] = "Job Title deleted successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    //error message - job title not deleted
                    TempData["errorMsg"] = "Error occurred. Job Title not deleted. ";
                    return RedirectToAction("list");
                }

            }
            else
            {
                // requested Job Title not found
                TempData["errorMsg"] = "Job Title does not exist.";
                return RedirectToAction("List");
            }
        }

    }
}
