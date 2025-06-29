using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.Options;
using MCQInterviews.Repositories.Questions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCQInterviews.Controllers
{
    [Authorize]
    public class OptionController : Controller
    {
        private readonly IOptionRepository _optionRepository;
        private readonly IQuestionRepository _questionRepository;

        public OptionController(IOptionRepository optionRepository, IQuestionRepository questionRepository)
        {
            _optionRepository = optionRepository;
            _questionRepository = questionRepository;
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> Add(int questionId)
        {
            // Validate input
            if (questionId <= 0)
            {
                return BadRequest("Invalid question ID.");
            }

            var option = new QuestionOption();
            option.QuestionId = questionId;
            ViewBag.QuestionId = questionId;

            // Find the selected Question text  
            var selectedQuestion = await _questionRepository.GetQuestionByIdAsync(questionId);
            var questionText = selectedQuestion?.Text;
            ViewBag.QuestionText = questionText;


            return View(option);
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int questionId, QuestionOption option)

        {
            // Validate inputs
            if (questionId <= 0 || option == null)
            {
                return BadRequest("Invalid parameters.");
            }
            option.QuestionId = questionId;
            option.IsCorrect = false;

            // Add the option to the database
            var result = await _optionRepository.AddOptionAsync(questionId, option);

            if (result != null)
            {
                TempData["successMsg"] = "Option added successfully";
                return RedirectToAction("List", new { questionId });
            }
            else
            {
                TempData["errorMsg"] = "Error occurred. Option not added ";
                return View(option);
            }

        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var optionToEdit = await _optionRepository.GetOptionByIdAsync(id);
            if (optionToEdit == null || string.IsNullOrEmpty(optionToEdit.Text))
            {
                return NotFound();
            }
            var option = new EditOptionRequest
            {
                Id = optionToEdit.Id,
                Text = optionToEdit.Text,
                QuestionId = optionToEdit.QuestionId,
                IsCorrect = optionToEdit.IsCorrect

            };
            return View(option);

        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditOptionRequest editOptionRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(editOptionRequest);
            }
            if (editOptionRequest == null)
            {
                var notFoundResult = new NotFoundObjectResult("Error occured. The requested Question Option was not found.");
                notFoundResult.StatusCode = StatusCodes.Status404NotFound;
                return notFoundResult;
            }

            var editedOption = await _optionRepository.GetOptionByIdAsync(editOptionRequest.Id);
            if (editedOption == null || string.IsNullOrEmpty(editOptionRequest.Text))
            {
                var notFoundResult = new NotFoundObjectResult("Error occured. The requested Question Option was not found.");
                notFoundResult.StatusCode = StatusCodes.Status404NotFound;
                return notFoundResult;
            }
            if (editedOption != null)
            {
                var updatedOption = new QuestionOption
                {
                    Id = editOptionRequest.Id,
                    Text = editOptionRequest.Text
                };
                var optionToUpdate = await _optionRepository.UpdateOptionAsync(updatedOption);

                if (optionToUpdate != null)
                {
                    TempData["successMsg"] = "Question Option updated successfully";
                }

                return RedirectToAction("List", new { questionId = editOptionRequest.QuestionId });
            }
            else
            {
                TempData["errorMsg"] = "Error Occurred. Question Option doest not exist.";
                return View(editOptionRequest);
            }
        }






        //***** List the options of a question

        [HttpGet]
        public async Task<IActionResult> List(int questionId)
        {

            // Fetch question options and question text
            var questionOptions = await _optionRepository.GetOptionsAsync(questionId);
            var selectedQuestion = await _questionRepository.GetQuestionByIdAsync(questionId);
            var questionText = selectedQuestion?.Text;

            //Pass data to the view
            ViewBag.QuestionId = questionId;
            ViewBag.QuestionText = questionText;

            return View(questionOptions);
        }

        /*
         * Reset all options of a question before updating its "isCorrect" value
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetStatusForQuestion(int questionId)
        {
            // Ensure input validation
            if (questionId <= 0)
            {
                return BadRequest("Invalid question ID.");
            }
            await _optionRepository.ResetStatusForQuestionAsync(questionId);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOptionStatus(int optionId)
        {
            // Ensure input validation
            if (optionId <= 0)
            {
                return BadRequest("Invalid option ID.");
            }
            var option = await _optionRepository.GetOptionByIdAsync(optionId);

            if (option != null)
            {
                // Reset the status for the question using the repository
                await _optionRepository.ResetStatusForQuestionAsync(option.QuestionId);
                option.IsCorrect = true;
                // Set the status of the selected option to true
                await _optionRepository.UpdateOptionAsync(option);
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Option not found" });
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var optionToDelete = await _optionRepository.GetOptionByIdAsync(id);
            var questionId = optionToDelete.QuestionId;
            //If requested Job Level is found
            if (optionToDelete != null)
            {
                var deletedJobTitle = await _optionRepository.DeleteOptionAsync(optionToDelete);

                if (deletedJobTitle != null)
                {

                    TempData["successMsg"] = "Question Option deleted successfully.";
                    return RedirectToAction("List", new { questionId = questionId });
                }
                else
                {

                    TempData["errorMsg"] = "Error occurred. Question Option not deleted. ";
                    return RedirectToAction("list", new { questionId = questionId });
                }

            }
            else
            {
                // requested Option not found
                TempData["errorMsg"] = "Question Option does not exist.";
                return RedirectToAction("List", new { questionId = questionId });
            }
        }


    }
}
