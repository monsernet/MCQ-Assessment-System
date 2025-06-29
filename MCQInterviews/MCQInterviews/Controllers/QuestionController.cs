using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories;
using MCQInterviews.Repositories.DifficultyTypes;
using MCQInterviews.Repositories.ExcelImport;
using MCQInterviews.Repositories.JobLevels;
using MCQInterviews.Repositories.JobTitles;
using MCQInterviews.Repositories.McqQuestions;
using MCQInterviews.Repositories.MCQs;
using MCQInterviews.Repositories.Questions;
using MCQInterviews.Repositories.QuestionTypes;
using MCQInterviews.Repositories.ResponseTypes;
using MCQInterviews.Repositories.Themes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MCQInterviews.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IJobTitleRepository _jobTitleRepository;
        private readonly IJobLevelRepository _jobLevelRepository;
        private readonly IThemeRepository _themeRepository;
        private readonly IMCQRepository _mCQRepository;
        private readonly IMcqQuestionRepository _mcqQuestionRepository;
        private readonly IQuestionExcelImport _questionExcelImport;
        private readonly IQuestionDifficultyTypeRepository _questionDifficultyTypeRepository;
        private readonly IMcqDifficultyTypeRepository _mcqDifficultyTypeRepository;
        private readonly IQuestionTypeRepository _questionTypeRepository;
        private readonly IResponseTypeRepository _responseTypeRepository;

        public QuestionController(
            IQuestionRepository questionRepository,
            IJobTitleRepository jobTitleRepository,
            IJobLevelRepository jobLevelRepository,
            IThemeRepository themeRepository,
            IMCQRepository mCQRepository,
            IMcqQuestionRepository mcqQuestionRepository,
            IQuestionExcelImport questionExcelImport,
            IQuestionDifficultyTypeRepository questionDifficultyTypeRepository,
            IMcqDifficultyTypeRepository mcqDifficultyTypeRepository,
            IQuestionTypeRepository questionTypeRepository,
            IResponseTypeRepository responseTypeRepository
            )
        {
            _questionRepository = questionRepository;
            _jobTitleRepository = jobTitleRepository;
            _jobLevelRepository = jobLevelRepository;
            _themeRepository = themeRepository;
            _mCQRepository = mCQRepository;
            _mcqQuestionRepository = mcqQuestionRepository;
            _questionExcelImport = questionExcelImport;
            _questionDifficultyTypeRepository = questionDifficultyTypeRepository;
            _mcqDifficultyTypeRepository = mcqDifficultyTypeRepository;
            _questionTypeRepository = questionTypeRepository;
            _responseTypeRepository = responseTypeRepository;
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // Get Job Categories from Repository
            var themes = await _themeRepository.GetThemesAsync();
            var themeList = themes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            ViewBag.Themes = themeList;

            // Get Difficulty types from Repository
            var diffTypes = await _questionDifficultyTypeRepository.GetQuestionDifficultyTypesAsync();
            var diffTypeList = diffTypes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.TypeText }).ToList();
            ViewBag.DiffTypes = diffTypeList;

            // Get Question Types
            var questionTypes = await _questionTypeRepository.GetQuestionTypesAsync();
            var questionTypeList = questionTypes.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.TypeName
            }).ToList();
            ViewBag.QuestionTypes = questionTypeList;

            // Get Response Types from Repository
            var responseTypes = await _responseTypeRepository.GetResponseTypesAsync();
            var responseTypeList = responseTypes.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Name
            }).ToList();
            ViewBag.ResponseTypes = responseTypeList;

            return View();
        }

        

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddQuestionRequest addQuestionRequest)
        {
            // Initialize the question object
            var question = new Question
            {
                Text = addQuestionRequest.Text,
                JobTitleId = addQuestionRequest.JobTitleId,
                QuestionDifficultyTypeId = addQuestionRequest.DifficultyTypeId,
                QuestionTypeId = addQuestionRequest.QuestionTypeId,
                ResponseTypeId = addQuestionRequest.ResponseTypeId
            };

            // Handle different types of questions
            var questionType = await _questionTypeRepository.GetQuestionTypeByIdAsync(addQuestionRequest.QuestionTypeId);
            var questionTypeText = questionType.TypeName.ToLower();

            switch (questionTypeText)
            {
                case "mcq": // MCQ
                    if (addQuestionRequest.Options == null || !addQuestionRequest.Options.Any())
                    {
                        TempData["errorMsg"] = "MCQ questions must have options.";
                        await PopulateDropdownsAsync();
                        return View(addQuestionRequest);
                    }
                    break;

                case "text": // Text
                        // No special handling needed for text questions
                    break;

                case "photo": // Photo
                    if (string.IsNullOrEmpty(addQuestionRequest.PhotoUrl))
                    {
                        TempData["errorMsg"] = "Question must include a Photo.";
                        await PopulateDropdownsAsync();
                        return View(addQuestionRequest);
                    }
                    question.MediaUrl = addQuestionRequest.PhotoUrl; // Assuming you have this field in the Question model
                    break;

                case "video": // Video
                    if (string.IsNullOrEmpty(addQuestionRequest.VideoUrl))
                    {
                        TempData["errorMsg"] = "Question must include a Video.";
                        await PopulateDropdownsAsync();
                        return View(addQuestionRequest);
                    }
                    question.MediaUrl = addQuestionRequest.VideoUrl; // Assuming you have this field in the Question model
                    break;

                case "audio": // Audio
                    if (string.IsNullOrEmpty(addQuestionRequest.AudioUrl))
                    {
                        TempData["errorMsg"] = "Audio questions must include an audio URL.";
                        await PopulateDropdownsAsync();
                        return View(addQuestionRequest);
                    }
                    question.AudioUrl = addQuestionRequest.AudioUrl; // Assuming you have this field in the Question model
                    break;

                default:
                    TempData["errorMsg"] = "Invalid question type.";
                    await PopulateDropdownsAsync();
                    return View(addQuestionRequest);
            }

            // Handle response types if applicable
            var responseType = await _responseTypeRepository.GetResponseTypeByIdAsync(addQuestionRequest.ResponseTypeId);
            var responseTypeText = responseType.Name.ToLower();

            switch (responseTypeText)
            {
                case "mcq": // Response type is MCQ
                    if (addQuestionRequest.Options == null || !addQuestionRequest.Options.Any())
                    {
                        TempData["errorMsg"] = "MCQ response type must have options.";
                        await PopulateDropdownsAsync();
                        return View(addQuestionRequest);
                    }
                    question.Options = addQuestionRequest.Options.Select(o => new QuestionOption
                    {
                        Text = o.Text,
                        QuestionId = question.Id // Assuming this is set later when saving to the DB
                    }).ToList();
                    break;

                case "text": // Response type is Text
                             // No additional handling needed for text response type
                    break;

                case "audio": // Response type is Audio
                    if (string.IsNullOrEmpty(addQuestionRequest.AudioResponseType))
                    {
                        TempData["errorMsg"] = "Please select an audio response type (uploaded or live).";
                        await PopulateDropdownsAsync();
                        return View(addQuestionRequest);
                    }
                    question.AudioResponseType = addQuestionRequest.AudioResponseType; // Store the selected option
                    break;

                case "video": // Response type is Video
                    if (string.IsNullOrEmpty(addQuestionRequest.VideoResponseType))
                    {
                        TempData["errorMsg"] = "Please select a video response type (uploaded or live).";
                        await PopulateDropdownsAsync();
                        return View(addQuestionRequest);
                    }
                    question.VideoResponseType = addQuestionRequest.VideoResponseType; // Store the selected option
                    break;

                default:
                    TempData["errorMsg"] = "Unsupported response type.";
                    await PopulateDropdownsAsync();
                    return View(addQuestionRequest);
            }

            // Add the question to the repository
            var savedQuestion = await _questionRepository.AddQuestionAsync(question);

            

            // Now save the options only if the question type is MCQ
            if (questionTypeText == "mcq" && savedQuestion != null)
            {
                foreach (var option in addQuestionRequest.Options)
                {
                    var questionOption = new QuestionOption
                    {
                        Text = option.Text,
                        IsCorrect = option.IsCorrect,
                        QuestionId = savedQuestion.Id // Associate with the saved question
                    };
                    await _questionRepository.AddQuestionOptionAsync(questionOption);
                }
            }

            if (savedQuestion != null)
            {
                TempData["successMsg"] = "Question added successfully.";
                return RedirectToAction("List"); // Redirect to your preferred action
            }
            else
            {
                TempData["errorMsg"] = "An error occurred while adding the question.";
                await PopulateDropdownsAsync();
                return View(addQuestionRequest);
            }
        }

        private async Task PopulateDropdownsAsync()
        {
            // Get Job Categories from Repository
            var themes = await _themeRepository.GetThemesAsync();
            var themeList = themes.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Name
            }).ToList();
            ViewBag.Themes = themeList;

            // Get Difficulty types from Repository
            var diffTypes = await _questionDifficultyTypeRepository.GetQuestionDifficultyTypesAsync();
            var diffTypeList = diffTypes.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.TypeText
            }).ToList();
            ViewBag.DiffTypes = diffTypeList;

            // Get Question Types
            var questionTypes = await _questionTypeRepository.GetQuestionTypesAsync();
            var questionTypeList = questionTypes.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.TypeName
            }).ToList();
            ViewBag.QuestionTypes = questionTypeList;

            // Get Response Types from Repository
            var responseTypes = await _responseTypeRepository.GetResponseTypesAsync();
            var responseTypeList = responseTypes.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Name
            }).ToList();
            ViewBag.ResponseTypes = responseTypeList;
        }

        /* 
         * Get the job titles based on selected theme from dropdown list
         * */
        [HttpGet]
        public async Task<IActionResult> GetJobTitles(int themeId)
        {
            var jobTitles = await _jobTitleRepository.GetJobTitlesByThemeAsync(themeId);

            var jobTitleList = jobTitles.Select(j => new SelectListItem { Value = j.Id.ToString(), Text = j.Name }).ToList();
            return Json(jobTitleList);
        }



        /*
         * Get the list of the questions
         * */

        [HttpGet]
        public async Task<IActionResult> List()
        {

            var questions = await _questionRepository.GetQuestionsAsync();
            var questionViewModels = await MapQuestionsToViewModelsAsync(questions);

            return View(questionViewModels);

        }

        private async Task<List<QuestionViewModel>> MapQuestionsToViewModelsAsync(IEnumerable<Question> questions)
        {
            var questionViewModels = new List<QuestionViewModel>();

            foreach (var question in questions)
            {
                var jobTitleView = await _jobTitleRepository.GetJobTitleByIdAsync(question.JobTitleId);
                if (jobTitleView != null)
                {
                    var themeId = jobTitleView.ThemeId;
                    var themeView = await _themeRepository.GetThemeByIdAsync(themeId);
                    var themeName = themeView.Name;
                    var difficultyView = await _questionDifficultyTypeRepository.GetQuestionDifficultyTypeByIdAsync(question.QuestionDifficultyTypeId);
                    var difficultyTypeName = difficultyView.TypeText;
                    var viewModel = new QuestionViewModel
                    {
                        Id = question.Id,
                        Text = question.Text,
                        ThemeName = themeName,
                        JobTitleId = question.JobTitleId,
                        JobTitleName = jobTitleView.Name,
                        DifficultyTypeId = difficultyView.Id,
                        DifficultyTypeName = difficultyTypeName


                    };
                    questionViewModels.Add(viewModel);
                }

            }

            return questionViewModels;
        }

        /*
         * Get the details of a Question
         * */

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Get the MCQ by id
            var question = await _questionRepository.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            // Get Job Categories from the repository
            var themes = await _themeRepository.GetThemesAsync();
            var themeList = themes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            // Get Job Titles from the repository
            var jobTitles = await _jobTitleRepository.GetJobTitlesAsync();
            var jobTitleList = jobTitles.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            // Get Job Levels from the repository
            var jobLevels = await _jobLevelRepository.GetJobLevelsAsync();
            var jobLevelList = jobLevels.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            // Get all Difficulty Types from the repository
            var diffTypes = await _questionDifficultyTypeRepository.GetQuestionDifficultyTypesAsync();
            var diffTypeList = diffTypes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.TypeText }).ToList();
            // Get Job Category Id from JobTitle of the selected MCQ
            var jobTitleId = question.JobTitleId;
            var themeId = (await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId)).ThemeId;
            var themeName = (await _themeRepository.GetThemeByIdAsync(themeId)).Name;


            // Create the ViewModel
            var questionViewModel = new EditQuestionRequest
            {
                Id = question.Id,
                Text = question.Text,
                ThemeId = themeId,
                Themes = themeList,
                ThemeName = themeName,
                JobLevels = jobLevelList,
                JobTitleId = question.JobTitleId,
                JobTitles = jobTitleList,
                DifficultyTypeId = question.QuestionDifficultyTypeId,
                DifficultyTypes = diffTypeList
            };
            return View(questionViewModel);
        }

        /*
         * Save the updates made on the MCQ 
         * */

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditQuestionRequest editQuestionRequest)
        {
            
            var question = new Question
            {
                Id = editQuestionRequest.Id,
                Text = editQuestionRequest.Text,
                JobTitleId = editQuestionRequest.JobTitleId,
                QuestionDifficultyTypeId = editQuestionRequest.DifficultyTypeId


            };
            var updatedQuestion = await _questionRepository.UpdateQuestionAsync(question);
            if (updatedQuestion != null)
            {
                TempData["successMsg"] = "Question updated successfully";
                return RedirectToAction("List");
            }
            else
            {
                TempData["errorMsg"] = "Error occurred. Question not updated";
                return View(editQuestionRequest);
            }

        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Question question)
        {
            var questionToDelete = await _questionRepository.DeleteQuestionAsync(question);
            if (questionToDelete != null)
            {
                TempData["successMsg"] = "Question deleted successfully";
                return RedirectToAction("List");
            }
            else
            {
                TempData["errorMsg"] = "Error Occurred. Question not deleted.";
                return RedirectToAction("List");
            }
        }



        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> AddJobTitleQuestion(int jobTitleId, int jobLevelId)
        {
           
            if (jobTitleId <= 0 || jobLevelId <= 0)
            {
                return BadRequest("Job Title and/or Job Level not identified");
            }
            //Job Categories
            var themes = await _themeRepository.GetThemesAsync();
            var themeList = themes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            ViewBag.Themes = themeList;

            // Job Titles will be populated based on Theme(Category) selected item

            //Get Job Levels
            var jobLevels = await _jobLevelRepository.GetJobLevelsAsync();
            var jobLevelList = jobLevels.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            ViewBag.JobLevels = jobLevelList;

            var question = new Question();
            question.JobTitleId = jobTitleId;

            // Find the selected Job Title name  
            var selectedJobTitle = await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId);
            var selectedJobTitleName = selectedJobTitle?.Name;
            ViewBag.JobTitleName = selectedJobTitleName;
            var jobTitle = await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId);
            var themeId = jobTitle.ThemeId;
            ViewBag.SelectedTheme = themeId;
            //Find the selected Job Category name based on the job title's ThemeId
            var selectedTheme = await _themeRepository.GetThemeByIdAsync(themeId);
            var selectedThemeName = selectedTheme?.Name;
            ViewBag.ThemeName = selectedThemeName;

            return View(question);
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddJobTitleQuestion(int jobTitleId, Question question)

        {
            
            if (jobTitleId <= 0 || question == null)
            {
                return BadRequest("Job Title  or Question not identified");
            }
            question.JobTitleId = jobTitleId;

            // Add the question to the database
            var result = await _questionRepository.AddJobTitleQuestion(jobTitleId, question);

            if (result != null)
            {
                TempData["successMsg"] = "Question added successfully";
                return RedirectToAction("QuestionList", "Mcq", new { jobTitleId });
            }
            else
            {
                TempData["errorMsg"] = "Error occurred. Question not added ";
                return View(question);
            }

        }

        // Add Bulk Questions
        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> AddJobTitleBulkQuestions()
        {
            //Job Categories
            var themes = await _themeRepository.GetThemesAsync();
            var themeList = themes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            ViewData["Themes"] = themeList;

            // Job Titles will be populated based on Theme(Category) selected item

            //Get Job Levels  from Repository
            var jobLevels = await _jobLevelRepository.GetJobLevelsAsync();
            var jobLevelList = jobLevels.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            ViewBag.JobLevels = jobLevelList;

            //Get Difficulty Types from Repository
            var diffTypes = await _questionDifficultyTypeRepository.GetQuestionDifficultyTypesAsync();
            var diffTypesList = diffTypes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.TypeText }).ToList();
            ViewBag.DiffTypes = diffTypesList;



            var model = new BulkQuestionExcelViewModel();

            return View(model);
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddJobTitleBulkQuestions(int jobTitleId, int jobLevelId, int difficultyTypeId, IFormFile file)
        {


            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please upload a file.");
                return View();
            }

            try
            {
                
                var themes = await _themeRepository.GetThemesAsync();
                var themeList = themes.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();

                var jobLevels = await _jobLevelRepository.GetJobLevelsAsync();
                var jobLevelList = jobLevels.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();

                //Pass data to the view
                ViewData["Themes"] = themeList;
                ViewBag.JobLevels = jobLevelList;

                using (var stream = file.OpenReadStream())
                {
                    // Read questions from Excel and save them to the database
                    var bulkQuestions = await _questionExcelImport.ReadQuestionsFromExcel(stream, jobTitleId, jobLevelId, difficultyTypeId);

                    TempData["successMsg"] = "Bulk questions added successfully";
                    return RedirectToAction("List", "Question");
                }
            }
            catch (Exception ex)
            {

                TempData["errorMsg"] = $"Outer Exception: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["errorMsg"] += $"Inner Exception: {ex.InnerException.Message}";
                }
                throw;
            }

        }


        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpGet]
        public async Task<IActionResult> AddMcqQuestion(int mcqId, int mcqDiffTypeId, string allocationData)
        {
            var mcq = await _mCQRepository.GetMCQByIdAsync(mcqId);
            //Get JobTitleName
            var jobTitleId = mcq.JobTitleId;
            var jobTitle = await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId);
            var jobTitleName = jobTitle.Name;
            //Get JobLevelName
            var jobLevelId = mcq.JobLevel;
            var jobLevel = await _jobLevelRepository.GetJobLevelByIdAsync(jobLevelId);
            var jobLevelName = jobLevel.Name;
            //Get MCQ Name
            var mcqName = mcq.Name;
            // Get all Difficulty Types from the repository
            var diffTypes = await _questionDifficultyTypeRepository.GetQuestionDifficultyTypesAsync();
            var diffTypeList = diffTypes.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.TypeText,
                Selected = m.Id == mcqDiffTypeId
            }).ToList();
            //Display Jobtitle, JobLevel and MCQ
            ViewBag.jobTitleId = jobTitleId;
            ViewBag.jobLevelId = jobLevelId;
            ViewBag.mcqId = mcqId;
            ViewBag.jobTitleName = jobTitleName;
            ViewBag.jobLevelName = jobLevelName;
            ViewBag.mcqName = mcqName;
            ViewBag.difficultyList = diffTypeList;

            // Deserialize the allocationData
            var allocations = JsonConvert.DeserializeObject<List<DifficultyAllocationViewModel>>(allocationData);
            ViewBag.Allocations = allocations;
            ViewBag.McqDiffTypeId = mcqDiffTypeId;

            return View();
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMcqQuestion(int mcqId, int mcqDiffTypeId, Question question, string allocationData)
        {

            /*
             * We should first make sure that we did not exceed the allocated number of question for the selected Difficulty Type
             * */
            var allocations = JsonConvert.DeserializeObject<List<DifficultyAllocationViewModel>>(allocationData);
            var selectedAllocation = allocations.FirstOrDefault(a => a.QuestionDifficultyTypeId == question.QuestionDifficultyTypeId);

            if (selectedAllocation != null && selectedAllocation.CurrentQuestionCount >= selectedAllocation.NbQuestions)
            {
                TempData["errorMsg"] = "Cannot add the new question. The allocated number of questions for the selected Difficulty Type has been reached.";
                return RedirectToAction("AddMcqQuestion", new { mcqId, mcqDiffTypeId, allocationData });
            }

            var mcq = await _mCQRepository.GetMCQByIdAsync(mcqId);

            if (mcq != null)
            {
                var jobTitleId = mcq.JobTitleId;

                // Add the question to the database
                var addedQuestion = await _questionRepository.AddJobTitleQuestion(jobTitleId, question);
                if (addedQuestion != null)
                {
                    // Retrieve the questionId after adding the question
                    var questionId = addedQuestion.Id;

                    // Add the questionId and mcqId to the McqQuestion entity
                    var mcqQuestionResult = await _mcqQuestionRepository.AddMcqQuestionWithQuestionIdAsync(mcqId, questionId);

                    if (mcqQuestionResult != null)
                    {
                        TempData["successMsg"] = "Question added to MCQ successfully";
                        return RedirectToAction("List", "Option", new { questionId });
                    }
                    else
                    {
                        TempData["errorMsg"] = "Error occurred. Question not added to MCQ.";
                        return RedirectToAction("QuestionList", "McqQuestion", new { mcqId, jobTitleId });
                    }
                }
                else
                {
                    TempData["errorMsg"] = "Error occurred. Question not added.";
                    return View(question);
                }
            }
            else
            {
                TempData["errorMsg"] = "Error occurred. MCQ not found";
                return RedirectToAction("QuestionList", "McqQuestion", new { mcqId });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetExistingQuestions(int mcqId, int jobTitleId, int jobLevelId)
        {
            var mcq = await _mCQRepository.GetMCQByIdAsync(mcqId);
            ViewBag.McqName = (mcq != null) ? mcq.Name : "NA";
            ViewBag.McqId = mcqId;
            var jobTitle = await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId);
            ViewBag.JobTitleId = jobTitleId;
            ViewBag.JobTitleName = (jobTitle != null) ? jobTitle.Name : "NA";
            var jobLevel = await _jobLevelRepository.GetJobLevelByIdAsync(jobLevelId);
            ViewBag.JobLevelId = jobLevelId;
            ViewBag.JobLevelName = (jobLevel != null) ? jobLevel.Name : "NA";
            ViewBag.McqDiffTypeId = mcq.McqDifficultyTypeId;
            ViewBag.McqDiffTypeName = (await _mcqDifficultyTypeRepository.GetMcqDifficultyTypeByIdAsync(mcq.McqDifficultyTypeId))?.TypeText;


            //Get questions of the selected JobTitle and JobLevel
            var questions = await _questionRepository.GetExistingQuestions(mcq.Id, jobTitleId, jobLevelId);


            var questionsViewModel = new List<QuestionViewModel>();
            foreach (var question in questions)
            {

                var questionDiffTypeName = (await _questionDifficultyTypeRepository.GetQuestionDifficultyTypeByIdAsync(question.QuestionDifficultyTypeId))?.TypeText;
                var ViewModel = new QuestionViewModel()
                {
                    Id = question.Id,
                    Text = question.Text,
                    DifficultyTypeName = questionDiffTypeName ?? "N/A"

                };
                questionsViewModel.Add(ViewModel);
            }
            return View(questionsViewModel);
        }


    }
}
