using AuthSystem.Areas.Identity.Data;
using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.DifficultyTypes;
using MCQInterviews.Repositories.JobLevels;
using MCQInterviews.Repositories.JobTitles;
using MCQInterviews.Repositories.McqQuestions;
using MCQInterviews.Repositories.MCQs;
using MCQInterviews.Repositories.MCQTestResults;
using MCQInterviews.Repositories.Options;
using MCQInterviews.Repositories.Questions;
using MCQInterviews.Repositories.Themes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static MCQInterviews.Enums.Enums;

namespace MCQInterviews.Controllers
{
    public class McqController : Controller
    {
        private readonly IMCQRepository _mCQRepository;
        private readonly IThemeRepository _themeRepository;
        private readonly IJobTitleRepository _jobTitleRepository;
        private readonly IJobLevelRepository _jobLevelRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IMcqQuestionRepository _mcqQuestionRepository;
        private readonly IOptionRepository _optionRepository;
        private readonly IMCQTestResultRepository _mCQTestResultRepository;
        private readonly IMcqDifficultyTypeRepository _mcqDifficultyTypeRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        private static readonly Random random = new ();

        public McqController(
            IMCQRepository mCQRepository,
            IThemeRepository themeRepository,
            IJobTitleRepository jobTitleRepository,
            IJobLevelRepository jobLevelRepository,
            IQuestionRepository questionRepository,
            IMcqQuestionRepository mcqQuestionRepository,
            IOptionRepository optionRepository,
            IMCQTestResultRepository mCQTestResultRepository,
            IMcqDifficultyTypeRepository mcqDifficultyTypeRepository,
            UserManager<ApplicationUser> userManager
            )
        {
            _mCQRepository = mCQRepository;
            _themeRepository = themeRepository;
            _jobTitleRepository = jobTitleRepository;
            _jobLevelRepository = jobLevelRepository;
            _questionRepository = questionRepository;
            _mcqQuestionRepository = mcqQuestionRepository;
            _optionRepository = optionRepository;
            _mCQTestResultRepository = mCQTestResultRepository;
            _mcqDifficultyTypeRepository = mcqDifficultyTypeRepository;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {

            
            var themes = await _themeRepository.GetThemesAsync();
            var themeList = themes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            ViewBag.Themes = themeList;

            // Job Titles will be populated based on selected Job Category

            //Get Job Levels  from Repository
            var jobLevels = await _jobLevelRepository.GetJobLevelsAsync();
            var jobLevelList = jobLevels.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            ViewBag.JobLevels = jobLevelList;
            //Get Difficulty Types  from Repository
            var diffTypes = await _mcqDifficultyTypeRepository.GetMcqDifficultyTypesAsync();
            var diffTypeList = diffTypes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.TypeText }).ToList();
            ViewBag.DiffTypes = diffTypeList;
            // Add MCQType options to the ViewBag
            var mcqTypeOptions = Enum.GetValues(typeof(MCQType))
                                     .Cast<MCQType>()
                                     .Select(m => new SelectListItem { Value = m.ToString(), Text = m.ToString() })
                                     .ToList();
            ViewBag.MCQTypes = mcqTypeOptions;

            return View();
        }

        //**** Get the job titles based on selected theme from dropdown list
        [HttpGet]
        public async Task<IActionResult> GetJobTitles(int themeId)
        {
            var jobTitles = await _jobTitleRepository.GetJobTitlesByThemeAsync(themeId);

            var jobTitleList = jobTitles.Select(j => new SelectListItem { Value = j.Id.ToString(), Text = j.Name }).ToList();
            return Json(jobTitleList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddMcqRequest addMcqRequest)
        {
            var mcq = new MCQ
            {
                Name = addMcqRequest.Name,
                Description = addMcqRequest.Description,
                Duration = addMcqRequest.Duration,
                NbQuestions = addMcqRequest.NbQuestions,
                MCQType = addMcqRequest.MCQType,
                JobTitleId = addMcqRequest.JobTitleId,
                JobLevel = addMcqRequest.JobLevelId,
                McqDifficultyTypeId = addMcqRequest.DifficultyTypeId
            };
            await _mCQRepository.AddMCQAsync(mcq);
            TempData["successMsg"] = "MCQ added successfully";
            return RedirectToAction("List");
        }

        //**** Get the list of  MCQs
        [HttpGet]
        public async Task<IActionResult> List(int? categoryId, int? jobTitleId)
        {

            IEnumerable<MCQ> mcqs;
            if (categoryId.HasValue && jobTitleId.HasValue && jobTitleId.Value != 0)
            {
                mcqs = await _mCQRepository.GetMCQsByCategoryAndJobTitleAsync(categoryId.Value, jobTitleId.Value);
            }
            else if (categoryId.HasValue)
            {
                mcqs = await _mCQRepository.GetMCQsByCategoryAsync(categoryId.Value);
            }
            else
            {
                mcqs = await _mCQRepository.GetMCQsAsync();
            }

            var mcqViewModels = await MapMcqsToViewModelsAsync(mcqs);

            // Get job categories
            var categories = await _themeRepository.GetThemesAsync();
            var categoryList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            // Get job titles for the selected category
            var jobTitles = new List<SelectListItem>();
            if (categoryId.HasValue)
            {
                var titles = await _jobTitleRepository.GetJobTitlesByThemeAsync(categoryId.Value);
                jobTitles = titles.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList();
            }

            ViewBag.JobCategories = categoryList;
            ViewBag.JobTitles = jobTitles;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedJobTitleId = jobTitleId;

            return View(mcqViewModels);

        }

        private async Task<List<McqViewModel>> MapMcqsToViewModelsAsync(IEnumerable<MCQ> mcqs)
        {
            var mcqViewModels = new List<McqViewModel>();

            foreach (var mcq in mcqs)
            {
                var jobTitleView = await _jobTitleRepository.GetJobTitleByIdAsync(mcq.JobTitleId);

                var jobLevelView = await _jobLevelRepository.GetJobLevelByIdAsync(mcq.JobLevel);

                var diffTypeView = await _mcqDifficultyTypeRepository.GetMcqDifficultyTypeByIdAsync(mcq.McqDifficultyTypeId);

                var questionCount = await _mcqQuestionRepository.CountMcqQuestionsAsync(mcq.Id);

                if (jobTitleView != null)
                {
                    var themeId = jobTitleView.ThemeId;
                    var themeView = await _themeRepository.GetThemeByIdAsync(themeId);
                    var themeName = themeView.Name ?? "N/A";

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
                        JobTitleName = jobTitleView.Name ?? "N/A",
                        JobLevelId = mcq.JobLevel,
                        JobLevelName = jobLevelView?.Name ?? "N/A",
                        DifficultyTypeId = mcq.McqDifficultyTypeId,
                        DifficultyTypeName = diffTypeView?.TypeText ?? "N/A",
                        AddedQuestions = questionCount
                    };
                    mcqViewModels.Add(viewModel);
                }

            }

            return mcqViewModels;
        }

        //***** Get the details of an MCQ
        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Get the MCQ by id
            var mcq = await _mCQRepository.GetMCQByIdAsync(id);
            if (mcq == null)
            {
                return NotFound();
            }
            //Job Categories
            var themes = await _themeRepository.GetThemesAsync();
            var themeList = themes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            //Job Titles
            var jobTitles = await _jobTitleRepository.GetJobTitlesAsync();
            var jobTitleList = jobTitles.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            //Job Levels
            var jobLevels = await _jobLevelRepository.GetJobLevelsAsync();
            var jobLevelList = jobLevels.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            //Difficulty Types
            var diffTypes = await _mcqDifficultyTypeRepository.GetMcqDifficultyTypesAsync();
            var diffTypeList = diffTypes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.TypeText }).ToList();
            //Job Category Id
            var jobTitleId = mcq.JobTitleId;
            var themeId = (await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId)).ThemeId;
            

            var mcqTypeOptions = Enum.GetValues(typeof(MCQType))
                                     .Cast<MCQType>()
                                     .Select(m => new SelectListItem { Value = m.ToString(), Text = m.ToString() })
                                     .ToList();


            // Create the ViewModel
            var mcqViewModel = new EditMcqRequest
            {
                Id = mcq.Id,
                Name = mcq.Name,
                Description = mcq.Description,
                Duration = mcq.Duration,
                NbQuestions = mcq.NbQuestions,
                MCQTypeOptions = mcqTypeOptions,
                MCQType = mcq.MCQType,
                ThemeId = themeId,
                Themes = themeList,
                JobLevelId = mcq.JobLevel,
                JobLevels = jobLevelList,
                JobTitleId = mcq.JobTitleId,
                JobTitles = jobTitleList,
                DifficultyTypeId = mcq.McqDifficultyTypeId,
                DifficultyTypes = diffTypeList
            };
            return View(mcqViewModel);
        }

        //**** Save the updates made on the MCQ 
        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditMcqRequest editMcqRequest)
        {

            var mcq = new MCQ
            {
                Id = editMcqRequest.Id,
                Name = editMcqRequest.Name,
                Description = editMcqRequest.Description,
                Duration = editMcqRequest.Duration,
                NbQuestions = editMcqRequest.NbQuestions,
                MCQType = editMcqRequest.MCQType,
                JobTitleId = editMcqRequest.JobTitleId,
                JobLevel = editMcqRequest.JobLevelId,
                McqDifficultyTypeId = editMcqRequest.DifficultyTypeId

            };
            var updatedMcq = await _mCQRepository.UpdateMCQAsync(mcq);
            if (updatedMcq != null)
            {
                TempData["SuccessMsg"] = "MCQ updated successfully";
                return RedirectToAction("List");
            }
            else
            {
                TempData["AlertMsg"] = "Error occurred. MCQ not updated";
                return View(editMcqRequest);
            }

        }

        //**** Delete MCQ 
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var mcqToDelete = await _mCQRepository.GetMCQByIdAsync(id);
            //If requested MCQ is found
            if (mcqToDelete != null)
            {
                var deletedMcq = await _mCQRepository.DeleteMCQAsync(mcqToDelete);
                if (deletedMcq != null)
                {
                    // success message - MCQ deleted
                    TempData["successMsg"] = "MCQ deleted successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    //error message - MCQ not deleted
                    TempData["errorMsg"] = "Error occurred. MCQ not deleted. ";
                    return RedirectToAction("list");
                }

            }
            else
            {
                // requested MCQ not found
                TempData["errorMsg"] = "MCQ does not exist.";
                return RedirectToAction("List");
            }
        }

        public async Task<IActionResult> DisplayMCQTest(int mcqId)
        {
            // Get MCQ questions with options
            var mcqQuestionsWithOptions = await _mcqQuestionRepository.GetMcqQuestionsWithOptionsAsync(mcqId);
            var mcq = await _mCQRepository.GetMCQByIdAsync(mcqId);
            var mcqTiming = mcq.Duration;
            ViewBag.McqDuration = mcqTiming;
            ViewBag.McqId = mcqId;
            // Check if there are any questions
            if (mcqQuestionsWithOptions == null || !mcqQuestionsWithOptions.Any())
            {
                TempData["errorMsg"] = "No questions found for the specified MCQ.";
                return RedirectToAction("List");
            }
            // Randomize the list of questions
            mcqQuestionsWithOptions = Randomize(mcqQuestionsWithOptions);
            return View(mcqQuestionsWithOptions);
        }

        // Radomize the list of question of the displayed Test
        public static List<T> Randomize<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        //Randomize the list of Optios for each question 
        public static void RandomizeOptions(McqQuestionWithOptionsViewModel mcqQuestion)
        {
            if (mcqQuestion.Options == null)
            {
                return;
            }

            var optionsList = mcqQuestion.Options
                .Select(option => option.OptionText)
                .ToList();

            optionsList = Randomize(optionsList);

            // Now update the original collection with the randomized options
            var index = 0;
            foreach (var option in mcqQuestion.Options)
            {
                option.OptionText = optionsList[index++];
            }
        }

        public async Task<IActionResult> PreviewMCQTest(int mcqId)
        {
            // Get MCQ questions with options
            var mcqQuestionsWithOptions = await _mcqQuestionRepository.GetMcqQuestionsWithOptionsAsync(mcqId);
            var mcq = await _mCQRepository.GetMCQByIdAsync(mcqId);
            var mcqTiming = mcq.Duration;
            ViewBag.McqDuration = mcqTiming;
            ViewBag.McqId = mcqId;
            // Check if there are any questions
            if (mcqQuestionsWithOptions == null || !mcqQuestionsWithOptions.Any())
            {
                TempData["errorMsg"] = "No questions found for the specified MCQ.";
                return RedirectToAction("List");
            }
            // Randomize the list of questions
            mcqQuestionsWithOptions = Randomize(mcqQuestionsWithOptions);
            return View(mcqQuestionsWithOptions);
        }



        //Submit the answered test
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessAnswer(int McqId, List<int> SelectedOptions, List<int> QuestionIds)
        {

            // Retrieve the correct options for the questions from the database
            var correctOptions = await GetCorrectOptions(QuestionIds);

            // Calculate the number of correct and wrong answers
            int correctCount = 0;
            int wrongCount = 0;
            int totalPoints = 0;

            // List to store the results for each question
            List<QuestionResultViewModel> questionResults = new List<QuestionResultViewModel>();

            for (int i = 0; i < QuestionIds.Count; i++)
            {
                int questionId = QuestionIds[i];
                //Extract the question text to be displayed in the view
                var question = await _questionRepository.GetQuestionByIdAsync(questionId);
                var questionText = question.Text;

                // Check if the index is within the range of SelectedOptions _
                // to avoid the out of range error
                if (i < SelectedOptions.Count)
                {
                    int selectedOptionId = SelectedOptions[i];

                    // Check if the selected option is correct
                    bool isCorrect = correctOptions.TryGetValue(questionId, out var correctOption) && selectedOptionId == correctOption.OptionId;

                    // If the question is unanswered (selectedOptionId == 0), consider it incorrect
                    if (selectedOptionId == 0)
                    {
                        isCorrect = false;
                    }

                    // Update counts
                    if (isCorrect)
                    {
                        correctCount++;
                        // Calculate points based on the difficulty type
                        int difficultyPoints = await _mcqQuestionRepository.GetDifficultyPointsAsync(questionId);
                        totalPoints += difficultyPoints;
                    }
                    else
                    {
                        wrongCount++;
                    }

                    // Add the result for each question to the list
                    questionResults.Add(new QuestionResultViewModel
                    {
                        QuestionId = questionId,
                        QuestionText = questionText,
                        SelectedOptionId = selectedOptionId,
                        IsCorrect = isCorrect,
                        SelectedOptionText = await GetOptionText(selectedOptionId),
                        CorrectOptionText = correctOption?.Text
                    });
                }
                else
                {
                    
                    questionResults.Add(new QuestionResultViewModel
                    {
                        QuestionId = questionId,
                        QuestionText = questionText,
                        IsCorrect = false,
                        SelectedOptionText = "No option selected",
                        CorrectOptionText = correctOptions.TryGetValue(questionId, out var correctOption) ? correctOption.Text : "Correct option not available"

                    });

                    // Increment wrongCount for unanswered questions
                    wrongCount++;
                }
            }

            // Calculate the percentage
            double percentage = (double)correctCount / QuestionIds.Count * 100;

            var mcqTestResult = new MCQTestResult
            {
                UserId = _userManager.GetUserId(User),
                MCQId = McqId,
                Score = (int)percentage,
                Points = totalPoints,
                DateTaken = DateTime.Now

            };

            // Save the MCQTestResult in the database
            await _mCQTestResultRepository.AddMCQTestResultAsync(mcqTestResult);

            // Pass the results to be displayed in the view 
            ViewData["CorrectCount"] = correctCount;
            ViewData["WrongCount"] = wrongCount;
            ViewData["Percentage"] = percentage;
            ViewData["TotalPoints"] = totalPoints;
            ViewData["QuestionResults"] = questionResults;


            return View("TestResults", questionResults);
        }

       
        // Get correct options for each question from the database
        private async Task<Dictionary<int, OptionTestViewModel>> GetCorrectOptions(List<int> questionIds)
        {
            // Retrieve correct options from database
            Dictionary<int, OptionTestViewModel> correctOptions = new Dictionary<int, OptionTestViewModel>();

            // Returns the correct option for each question
            foreach (var questionId in questionIds)
            {
                var correctOptionsList = await _optionRepository.GetCorrectOptionByQuestionId(questionId);

                foreach (var correctOption in correctOptionsList)
                {
                    if (correctOption != null)
                    {
                        correctOptions.Add(questionId, new OptionTestViewModel
                        {
                            OptionId = correctOption.Id,
                            Text = correctOption.Text,
                            IsCorrect = correctOption.IsCorrect
                        });
                    }
                }
            }

            return correctOptions;
        }

        // Get option text based on option ID
        private async Task<string> GetOptionText(int optionId)
        {
            // Retrieve option text from datanbase
            var optionText = await _optionRepository.GetOptionTextByOptionId(optionId);

            return optionText;
        }

        // Get the MCQs related to a specified JobTitle and JobLevel
        public async Task<IActionResult> ListByJobTitleAndLevel(int jobTitleId, int jobLevelId)
        {
           
            var validMcqs = await _mCQRepository.GetValidMCQsByJobTitleAndLevelAsync(jobTitleId, jobLevelId);

            var mcqViewModels = await MapMcqsToViewModelsAsync(validMcqs);

            return View("ListByJobTitleAndLevel", mcqViewModels);
        }



    }
}
