using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.DifficultyAllocations;
using MCQInterviews.Repositories.DifficultyTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCQInterviews.Controllers
{
    [Authorize(Roles = "Admin, Editor")]
    public class DifficultyAllocationController : Controller
    {
        private readonly IDifficultyAllocationRepository difficultyAllocationRepository;
        private readonly IMcqDifficultyTypeRepository mcqDifficultyTypeRepository;
        private readonly IQuestionDifficultyTypeRepository questionDifficultyTypeRepository;

        public DifficultyAllocationController(
            IDifficultyAllocationRepository difficultyAllocationRepository,
            IMcqDifficultyTypeRepository mcqDifficultyTypeRepository,
            IQuestionDifficultyTypeRepository questionDifficultyTypeRepository
            )
        {
            this.difficultyAllocationRepository = difficultyAllocationRepository;
            this.mcqDifficultyTypeRepository = mcqDifficultyTypeRepository;
            this.questionDifficultyTypeRepository = questionDifficultyTypeRepository;
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var mcqDiffTypes = await mcqDifficultyTypeRepository.GetMcqDifficultyTypesAsync();
            var mcqDiffTypeList = mcqDiffTypes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.TypeText }).ToList();
            ViewBag.McqDifficultyTypes = mcqDiffTypeList;

            var questionDiffTypes = await questionDifficultyTypeRepository.GetQuestionDifficultyTypesAsync();
            var questionDiffTypeList = questionDiffTypes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.TypeText }).ToList();
            ViewBag.QuestionDifficultyTypes = questionDiffTypeList;
            return View();
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddDifficultyAllocationRequest addDifficultyAllocationRequest)
        {
            if (ModelState.IsValid)
            {
                var allocation = new DifficultyAllocation()
                {
                    Id = addDifficultyAllocationRequest.Id,
                    Percentage = addDifficultyAllocationRequest.Percentage,
                    QuestionDifficultyTypeId = addDifficultyAllocationRequest.QuestionDifficultyTypeId,
                    McqDifficultyTypeId = addDifficultyAllocationRequest.McqDifficultyTypeId
                };
                await difficultyAllocationRepository.AddAsync(allocation);
            }

            return RedirectToAction("list");
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var allocations = await difficultyAllocationRepository.GetAllAsync();
            var groupedAllocations = await MapAllocationsToGroupedViewModelsAsync(allocations);

            return View(groupedAllocations);
        }

        private async Task<List<McqDifficultyTypeGroupViewModel>> MapAllocationsToGroupedViewModelsAsync(IEnumerable<DifficultyAllocation> allocations)
        {
            var groupedAllocations = new List<McqDifficultyTypeGroupViewModel>();

            var mcqDiffTypes = await mcqDifficultyTypeRepository.GetMcqDifficultyTypesAsync();
            var questionDiffTypes = await questionDifficultyTypeRepository.GetQuestionDifficultyTypesAsync();

            foreach (var mcqDiffType in mcqDiffTypes)
            {
                var mcqGroup = new McqDifficultyTypeGroupViewModel
                {
                    McqDifficultyTypeId = mcqDiffType.Id,
                    McqDifficultyTypeName = mcqDiffType.TypeText,
                    Allocations = questionDiffTypes.Select(qdt =>
                        new DifficultyAllocationViewModel
                        {
                            QuestionDifficultyTypeId = qdt.Id,
                            QuestionDifficultyTypeName = qdt.TypeText,
                            Percentage = allocations
                                .FirstOrDefault(a => a.McqDifficultyTypeId == mcqDiffType.Id && a.QuestionDifficultyTypeId == qdt.Id)?.Percentage ?? 0
                        }).ToList()
                };

                groupedAllocations.Add(mcqGroup);
            }

            return groupedAllocations;
        }



        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> GetDifficultyAllocations(int mcqDifficultyTypeId)
        {

            // Fetch all difficulty types
            var allDifficultyTypes = await questionDifficultyTypeRepository.GetQuestionDifficultyTypesAsync();

            // Fetch allocations for the selected MCQ difficulty type (if needed)
            var allocations = await difficultyAllocationRepository.GetAllocationsByMcqDifficultyTypeIdAsync(mcqDifficultyTypeId);

            // Combine data
            var modalData = allDifficultyTypes.Select(dt => new
            {
                questionDifficultyTypeId = dt.Id, // Use Id or appropriate identifier
                questionDifficultyTypeName = dt.TypeText,
                percentage = allocations.Any(a => a.QuestionDifficultyTypeId == dt.Id) ? allocations.FirstOrDefault(a => a.QuestionDifficultyTypeId == dt.Id)?.Percentage : 0
            }).ToList();

            return Json(modalData);


        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        public async Task<IActionResult> UpdateAllocations([FromBody] UpdateDifficultyAllocationsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var allocation in model.Allocations)
            {
                var existingAllocation = await difficultyAllocationRepository.GetByMcqAndQuestionTypeIdsAsync(model.McqDifficultyTypeId, allocation.QuestionDifficultyTypeId);

                if (existingAllocation != null)
                {
                    // Update existing allocation
                    existingAllocation.Percentage = allocation.Percentage;
                    await difficultyAllocationRepository.UpdateAsync(existingAllocation);
                }
                else
                {
                    // Create new allocation
                    var newAllocation = new DifficultyAllocation
                    {
                        McqDifficultyTypeId = model.McqDifficultyTypeId,
                        QuestionDifficultyTypeId = allocation.QuestionDifficultyTypeId,
                        Percentage = allocation.Percentage
                    };
                    await difficultyAllocationRepository.AddAsync(newAllocation);
                }
            }
            TempData["successMsg"] = "Allocations updated successfully.";
            return Ok();
        }




        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var allocationToDelete = await difficultyAllocationRepository.GetByIdAsync(id);

            if (allocationToDelete != null)
            {
                var deletedAllocation = await difficultyAllocationRepository.DeleteAsync(allocationToDelete);
                if (deletedAllocation != null)
                {
                    // success message
                    TempData["successMsg"] = "Difficulty Allocation deleted successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    //error message
                    TempData["errorMsg"] = "Error occurred. Difficulty Allocation not deleted. ";
                    return RedirectToAction("list");
                }

            }
            else
            {
                // requested Allocation not found
                TempData["errorMsg"] = "Difficulty Allocation does not exist.";
                return RedirectToAction("List");
            }
        }
    }
}
