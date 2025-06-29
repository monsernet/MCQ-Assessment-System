using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.DifficultyTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCQInterviews.Controllers
{
    [Authorize]
    public class QuestionDifficultyTypeController : Controller
    {
        private readonly IQuestionDifficultyTypeRepository _questionDifficultyTypeRepository;

        public QuestionDifficultyTypeController(IQuestionDifficultyTypeRepository questionDifficultyTypeRepository)
        {
            _questionDifficultyTypeRepository = questionDifficultyTypeRepository;
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
        public async Task<IActionResult> Add(AddQuestionDifficultyTypeRequest addQuestionDifficultyTypeRequest)
        {
            
            if (!ModelState.IsValid)
            {
                return View(addQuestionDifficultyTypeRequest);
            }
            var questionDiffType = new QuestionDifficultyType()
            {
                Id = addQuestionDifficultyTypeRequest.Id,
                TypeText = addQuestionDifficultyTypeRequest.TypeText,
                PointValue = addQuestionDifficultyTypeRequest.PointValue

            };
            await _questionDifficultyTypeRepository.AddQuestionDiffTypeAsync(questionDiffType);
            return RedirectToAction("list");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var questionDiffTypes = await _questionDifficultyTypeRepository.GetQuestionDifficultyTypesAsync();
            return View(questionDiffTypes);
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var questionDiffType = await _questionDifficultyTypeRepository.GetQuestionDifficultyTypeByIdAsync(id);
            if (questionDiffType != null)
            {
                var searchedquestionDiffType = new EditQuestionDifficultyTypeRequest()
                {
                    Id = questionDiffType.Id,
                    TypeText = questionDiffType.TypeText,
                    PointValue = questionDiffType.PointValue
                };
                return View(searchedquestionDiffType);
            }
            else
            {
                TempData[""] = "Question Difficulty Type does not exist";
                return RedirectToAction("List");
            }
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditQuestionDifficultyTypeRequest editQuestionDifficultyTypeRequest)
        {
            var questionDiffTypeToUpdate = await _questionDifficultyTypeRepository.GetQuestionDifficultyTypeByIdAsync(editQuestionDifficultyTypeRequest.Id);
            if (questionDiffTypeToUpdate != null)
            {
                var questionDiffType = new QuestionDifficultyType()
                {
                    Id = editQuestionDifficultyTypeRequest.Id,
                    TypeText = editQuestionDifficultyTypeRequest.TypeText,
                    PointValue = editQuestionDifficultyTypeRequest.PointValue
                };
                var updatedQuestionDiffType = await _questionDifficultyTypeRepository.UpdateQuestionDiffTypeAsync(questionDiffType);
                if (updatedQuestionDiffType != null)
                {
                    //update success message
                    TempData["successMsg"] = "Question Difficulty Type Updated successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    //update error message 
                    TempData["errorMsg"] = "Error occurred. Question Difficulty Type not updated.";
                    return RedirectToAction("List");
                }
            }
            else
            {
                // not found error message
                TempData["errorMsg"] = "The Question Difficulty Type does not exist.";
                return RedirectToAction("List");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var questionDiffTypeToDelete = await _questionDifficultyTypeRepository.GetQuestionDifficultyTypeByIdAsync(id);
            //If requested Job Level is found
            if (questionDiffTypeToDelete != null)
            {
                var deletedQuestionDiffType = await _questionDifficultyTypeRepository.DeleteQuestionDiffTypeAsync(questionDiffTypeToDelete);
                if (deletedQuestionDiffType != null)
                {
                    // success message - Job Level deleted
                    TempData["successMsg"] = "Question Difficulty Type deleted successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    //error message - job level not deleted
                    TempData["errorMsg"] = "Error occurred. Question Difficulty Type not deleted. ";
                    return RedirectToAction("list");
                }

            }
            else
            {
                // requested Job Level not found
                TempData["errorMsg"] = "Question Difficulty Type does not exist.";
                return RedirectToAction("List");
            }
        }

    }
}
