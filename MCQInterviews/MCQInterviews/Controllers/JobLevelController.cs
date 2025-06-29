using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.JobLevels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCQInterviews.Controllers
{
    [Authorize(Roles = "Admin, Editor")]
    public class JobLevelController : Controller
    {
        private readonly IJobLevelRepository _jobLevelRepository;

        public JobLevelController(IJobLevelRepository jobLevelRepository)
        {
            _jobLevelRepository = jobLevelRepository;
        }
        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddJobLevelRequest addJobLevelRequest)
        {
            var jobLevel = new JobLevel()
            {
                Id = addJobLevelRequest.Id,
                Name = addJobLevelRequest.Name
            };
            await _jobLevelRepository.AddJobLevelAsync(jobLevel);
            return RedirectToAction("list");
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var joblevels = await _jobLevelRepository.GetJobLevelsAsync();
            return View(joblevels);
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var jobLevel = await _jobLevelRepository.GetJobLevelByIdAsync(id);
            if (jobLevel != null)
            {
                var searchedJobLevel = new EditJobLevelRequest()
                {
                    Id = jobLevel.Id,
                    Name = jobLevel.Name
                };
                return View(searchedJobLevel);
            }
            else
            {
                TempData[""] = "Job Level does not exist";
                return RedirectToAction("List");
            }
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditJobLevelRequest editJobLevelRequest)
        {
            var jobLevelToUpdate = await _jobLevelRepository.GetJobLevelByIdAsync(editJobLevelRequest.Id);
            if (jobLevelToUpdate != null)
            {
                var jobLevel = new JobLevel()
                {
                    Id = editJobLevelRequest.Id,
                    Name = editJobLevelRequest.Name
                };
                var updatedJobLevel = await _jobLevelRepository.UpdateJobLevelAsync(jobLevel);
                if (updatedJobLevel != null)
                {
                    //update success message
                    TempData["successMsg"] = "Job Level Updated successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    //update error message 
                    TempData["errorMsg"] = "Error occurred. Job Level not updated.";
                    return RedirectToAction("List");
                }
            }
            else
            {
                // not found error message
                TempData["errorMsg"] = "The Job Level does not exist.";
                return RedirectToAction("List");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var jobTitleToDelete = await _jobLevelRepository.GetJobLevelByIdAsync(id);
            //If requested Job Level is found
            if (jobTitleToDelete != null)
            {
                var deletedJobTitle = await _jobLevelRepository.DeleteJobLevelAsync(jobTitleToDelete);
                if (deletedJobTitle != null)
                {
                    // success message - Job Level deleted
                    TempData["successMsg"] = "Job Level deleted successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    //error message - job level not delete
                    TempData["errorMsg"] = "Error occurred. Job Level not deleted. ";
                    return RedirectToAction("list");
                }

            }
            else
            {
                // requested Job Level not found
                TempData["errorMsg"] = "Job Level does not exist.";
                return RedirectToAction("List");
            }
        }

    }
}
