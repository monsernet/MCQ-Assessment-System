using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.DifficultyTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCQInterviews.Controllers
{
    [Authorize]
    public class McqDifficultyTypeController : Controller
    {
        private readonly IMcqDifficultyTypeRepository _mcqDifficultyTypeRepository;

        public McqDifficultyTypeController(IMcqDifficultyTypeRepository mcqDifficultyTypeRepository)
        {
            _mcqDifficultyTypeRepository = mcqDifficultyTypeRepository;
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddMcqDifficultyTypeRequest addMcqDifficultyTypeRequest)
        {
            var mcqDiffType = new McqDifficultyType()
            {
                Id = addMcqDifficultyTypeRequest.Id,
                TypeText = addMcqDifficultyTypeRequest.TypeText
            };
            await _mcqDifficultyTypeRepository.AddMcqDiffTypeAsync(mcqDiffType);
            return RedirectToAction("list");
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var mcqDiffTypes = await _mcqDifficultyTypeRepository.GetMcqDifficultyTypesAsync();
            return View(mcqDiffTypes);
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var mcqDiffType = await _mcqDifficultyTypeRepository.GetMcqDifficultyTypeByIdAsync(id);
            if (mcqDiffType != null)
            {
                var searchedMcqDiffType = new EditMcqDifficultyTypeRequest()
                {
                    Id = mcqDiffType.Id,
                    TypeText = mcqDiffType.TypeText
                };
                return View(searchedMcqDiffType);
            }
            else
            {
                TempData[""] = "Mcq Difficulty Type does not exist";
                return RedirectToAction("List");
            }
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditMcqDifficultyTypeRequest editMcqDifficultyTypeRequest)
        {
            var mcqDiffTypeToUpdate = await _mcqDifficultyTypeRepository.GetMcqDifficultyTypeByIdAsync(editMcqDifficultyTypeRequest.Id);
            if (mcqDiffTypeToUpdate != null)
            {
                var mcqDiffType = new McqDifficultyType()
                {
                    Id = editMcqDifficultyTypeRequest.Id,
                    TypeText = editMcqDifficultyTypeRequest.TypeText
                };
                var updatedMcqDiffType = await _mcqDifficultyTypeRepository.UpdateMcqDiffTypeAsync(mcqDiffType);
                if (updatedMcqDiffType != null)
                {
                    //update success message
                    TempData["successMsg"] = "Mcq Difficulty Type Updated successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    //update error message 
                    TempData["errorMsg"] = "Error occurred. Mcq Difficulty Type not updated.";
                    return RedirectToAction("List");
                }
            }
            else
            {
                // not found error message
                TempData["errorMsg"] = "The Mcq Difficulty Type does not exist.";
                return RedirectToAction("List");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var mcqDiffTypeToDelete = await _mcqDifficultyTypeRepository.GetMcqDifficultyTypeByIdAsync(id);

            if (mcqDiffTypeToDelete != null)
            {
                var deletedMcqDiffType = await _mcqDifficultyTypeRepository.DeleteMcqDiffTypeAsync(mcqDiffTypeToDelete);
                if (deletedMcqDiffType != null)
                {
                    // success message 
                    TempData["successMsg"] = "MCQ Difficulty Type deleted successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    //error message 
                    TempData["errorMsg"] = "Error occurred. MCQ Difficulty Type not deleted. ";
                    return RedirectToAction("list");
                }

            }
            else
            {
                // requested Difficulty Type not found
                TempData["errorMsg"] = "MCQ Difficulty Type does not exist.";
                return RedirectToAction("List");
            }
        }

        public async Task<ActionResult<string>> GetMcqDifficultyTypeName(int mcqDifficultyTypeId)
        {
            var mcqDifficultyType = await _mcqDifficultyTypeRepository.GetMcqDifficultyTypeByIdAsync(mcqDifficultyTypeId);
            var mcqDifficultyTypeName = "N/A";
            if (mcqDifficultyType != null)
            {
                mcqDifficultyTypeName = mcqDifficultyType.TypeText;
            }
            return Json(mcqDifficultyTypeName);

        }
    }
}
