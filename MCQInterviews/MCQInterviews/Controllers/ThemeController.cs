using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.Themes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCQInterviews.Controllers
{
    [Authorize]
    public class ThemeController : Controller
    {
        private readonly IThemeRepository _themeRepository;

        public ThemeController(
            IThemeRepository themeRepository)
        {
            _themeRepository = themeRepository;
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
        public async Task<IActionResult> Add(AddThemeRequest addThemeRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(addThemeRequest);
            }

            var theme = new Theme
            {
                Name = addThemeRequest.Name
            };
            var addedTheme = await _themeRepository.AddThemeAsync(theme);
            if (addedTheme != null)
            {
                TempData["successMsg"] = "Job Category added successfully";
            }
            else
            {
                TempData["errorMsg"] = "Error occurred. Job Category not added.";
            }

            return RedirectToAction("List");
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var themes = await _themeRepository.GetThemesAsync();
            return View(themes);
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var themeToEdit = await _themeRepository.GetThemeByIdAsync(id);
            if (themeToEdit == null || string.IsNullOrEmpty(themeToEdit.Name))
            {
                return NotFound();
            }
            var theme = new EditThemeRequest
            {
                Id = themeToEdit.Id,
                Name = themeToEdit.Name
            };
            return View(theme);


        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditThemeRequest editThemeRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(editThemeRequest);
            }
            if (editThemeRequest == null)
            {
                var notFoundResult = new NotFoundObjectResult("Error occured. The requested Job Category was not found.");
                notFoundResult.StatusCode = StatusCodes.Status404NotFound;
                return notFoundResult;
            }

            var editedTheme = await _themeRepository.GetThemeByIdAsync(editThemeRequest.Id);
            if (editedTheme == null || string.IsNullOrEmpty(editThemeRequest.Name))
            {
                var notFoundResult = new NotFoundObjectResult("Error occured. The requested Job Category was not found.");
                notFoundResult.StatusCode = StatusCodes.Status404NotFound;
                return notFoundResult;
            }
            if (editedTheme != null)
            {
                var updatedTheme = new Theme
                {
                    Id = editThemeRequest.Id,
                    Name = editThemeRequest.Name
                };
                var updatedtheme = await _themeRepository.UpdateThemAsync(updatedTheme);

                if (updatedtheme != null)
                {
                    TempData["successMsg"] = "Theme updated successfully";
                }

                return RedirectToAction("List");
            }
            else
            {
                TempData["errorMsg"] = "Error Occurred. Theme doest not exist.";
                return View(editThemeRequest);
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Theme theme)
        {

            var themeToDelete = await _themeRepository.GetThemeByIdAsync(theme.Id);
            if (themeToDelete == null)
            {
                TempData["errorMsg"] = "Job Category not found";
                return RedirectToAction("List");
            }
            var deletedTheme = await _themeRepository.DeleteThemeAsync(themeToDelete);
            if (deletedTheme == null)
            {
                TempData["errorMsg"] = "Failed to delete Job Category";
            }
            else
            {
                TempData["successMsg"] = "Job Category deleted successfully";
            }

            return RedirectToAction("List");
        }
    }
}
