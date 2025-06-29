using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.DifficultyAllocations;
using MCQInterviews.Repositories.DifficultyTypes;
using MCQInterviews.Repositories.JobLevels;
using MCQInterviews.Repositories.JobTitles;
using MCQInterviews.Repositories.McqQuestions;
using MCQInterviews.Repositories.MCQs;
using MCQInterviews.Repositories.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace MCQInterviews.Controllers
{
    [Authorize]
    public class McqQuestionController : Controller
    {
        private readonly IMcqQuestionRepository _mcqQuestionRepository;
        private readonly IMCQRepository _mCQRepository;
        private readonly IJobLevelRepository _jobLevelRepository;
        private readonly IJobTitleRepository _jobTitleRepository;
        private readonly IMcqDifficultyTypeRepository _mcqDifficultyTypeRepository;
        private readonly IDifficultyAllocationRepository _difficultyAllocationRepository;
        private readonly IOptionRepository optionRepository;
        private static readonly Random random = new ();

        public McqQuestionController(
            IMcqQuestionRepository mcqQuestionRepository,
            IMCQRepository mCQRepository,
            IJobLevelRepository jobLevelRepository,
            IJobTitleRepository jobTitleRepository,
            IMcqDifficultyTypeRepository mcqDifficultyTypeRepository,
            IDifficultyAllocationRepository difficultyAllocationRepository,
            IOptionRepository optionRepository
            )
        {
            _mcqQuestionRepository = mcqQuestionRepository;
            _mCQRepository = mCQRepository;
            _jobLevelRepository = jobLevelRepository;
            _jobTitleRepository = jobTitleRepository;
            _mcqDifficultyTypeRepository = mcqDifficultyTypeRepository;
            _difficultyAllocationRepository = difficultyAllocationRepository;
            this.optionRepository = optionRepository;
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpGet]
        public async Task<IActionResult> QuestionList(int mcqId, int jobTitleId, int jobLevelId)
        {
            
            if (mcqId <= 0 || jobTitleId <= 0)
            {
                return BadRequest("Invalid parameters.");
            }
            // Retrieve MCQ and job title information
            var mcq = await _mCQRepository.GetMCQByIdAsync(mcqId);
            var jobTitle = await _jobTitleRepository.GetJobTitleByIdAsync(jobTitleId);
            // Ensure the requested MCQ and job title exist
            if (mcq == null || jobTitle == null)
            {
                return NotFound("MCQ, job title, or job level not found.");
            }
            //MCQ information
            ViewBag.McqName = mcq.Name;
            ViewBag.McqId = mcqId;
            //Job Title information
            ViewBag.JobTitleId = jobTitleId;
            ViewBag.JobTitleName = jobTitle.Name;
            //Job Level information
            ViewBag.JobLevelId = jobLevelId;

            //Difficulty Level information
            var mcqDiffTypeId = mcq.McqDifficultyTypeId;
            ViewBag.McqDiffTypeId = mcqDiffTypeId;
            ViewBag.McqDifficultyTypeName = (await _mcqDifficultyTypeRepository.GetMcqDifficultyTypeByIdAsync(mcqDiffTypeId))?.TypeText;
            //MCQ questions
            var questions = await _mcqQuestionRepository.GetMcqQuestionsAsync(mcqId);
            var questionCount = await _mcqQuestionRepository.CountMcqQuestionsAsync(mcqId);
            var mcqQuestions = mcq.NbQuestions;
            ViewBag.questionCount = questionCount;
            ViewBag.mcqQuestions = mcqQuestions;

            // Retrieve difficulty allocations for the specified MCQ Difficulty Type
            var difficultyAllocations = await _difficultyAllocationRepository.GetMcqDifficultyAllocations(mcqDiffTypeId);

            var questionCounts = _mcqQuestionRepository.CountQuestionsByDifficultyTypeAsync(mcqId);

            // Create a list to store difficulty allocation data
            var difficultyAllocationsData = new List<DifficultyAllocationViewModel>();
            var totalQuestionsForAllDifficultyTypes = 0;

            foreach (var allocation in difficultyAllocations)
            {

                totalQuestionsForAllDifficultyTypes += (int)(allocation.Percentage * mcqQuestions / 100);
                var difficultyTypeId = allocation.QuestionDifficultyTypeId.ToString();
                // Check if questionCounts contains the difficultyTypeId
                if (questionCounts.TryGetValue(difficultyTypeId, out var currQuestionCount))
                {


                    difficultyAllocationsData.Add(new DifficultyAllocationViewModel
                    {
                        QuestionDifficultyTypeId = allocation.QuestionDifficultyTypeId,
                        QuestionDifficultyTypeName = allocation.QuestionDifficultyType.TypeText,
                        Percentage = allocation.Percentage,
                        NbQuestions = (int)Math.Floor(allocation.Percentage * mcqQuestions / 100),
                        CurrentQuestionCount = currQuestionCount
                    });
                }
                else
                {
                    difficultyAllocationsData.Add(new DifficultyAllocationViewModel
                    {
                        QuestionDifficultyTypeId = allocation.QuestionDifficultyTypeId,
                        QuestionDifficultyTypeName = allocation.QuestionDifficultyType.TypeText,
                        Percentage = allocation.Percentage,
                        NbQuestions = (int)Math.Floor(allocation.Percentage * mcqQuestions / 100),
                        CurrentQuestionCount = 0
                    });
                }
            }
            if (mcqQuestions - totalQuestionsForAllDifficultyTypes > 0)
            {

                var allocationWithHighestPercentage = difficultyAllocationsData.OrderByDescending(allocation => allocation.Percentage).FirstOrDefault();
                var allocHighPercnbQuestions = allocationWithHighestPercentage.CurrentQuestionCount;
                var allocHighPercQuestDiffId = allocationWithHighestPercentage.QuestionDifficultyTypeId;
                var allocHighPercTypeName = allocationWithHighestPercentage.QuestionDifficultyTypeName;
                var allocHighPercPercentage = allocationWithHighestPercentage.Percentage;
                var allocHighPercNbQuestion = allocationWithHighestPercentage.NbQuestions;
                // Remove the allocation with the highest percentage from difficultyAllocationData
                var indexOfRemovedAllocation = difficultyAllocationsData.IndexOf(allocationWithHighestPercentage);
                difficultyAllocationsData.RemoveAt(indexOfRemovedAllocation);
                var newAllocation = new DifficultyAllocationViewModel
                {
                    QuestionDifficultyTypeId = allocHighPercQuestDiffId,
                    QuestionDifficultyTypeName = allocHighPercTypeName,
                    Percentage = allocHighPercPercentage,
                    NbQuestions = allocHighPercNbQuestion + 1,
                    CurrentQuestionCount = allocHighPercnbQuestions
                };
                difficultyAllocationsData.Insert(indexOfRemovedAllocation, newAllocation);
            }

            ViewBag.DifficultyAllocationsData = difficultyAllocationsData;

            //Check the number of options for each question
            foreach (var question in questions)
            {
                question.nbOptions = await optionRepository.OptionCountByQuestion(question.QuestionId);


            }

            return View(questions);

        }

        private static List<McqQuestionViewModel> ShuffleQuestions(List<McqQuestionViewModel> questions)
        {
            int n = questions.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                McqQuestionViewModel value = questions[k];
                questions[k] = questions[n];
                questions[n] = value;
            }
            return questions;
        }



        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSelectedQuestions(List<int> selectedQuestionIds, int mcqId, int jobTitleId, int jobLevelId)
        {
            // Ensure selectedQuestionIds is not null to avoid unhandeled exceptions
            if (selectedQuestionIds == null || selectedQuestionIds.Count == 0)
            {
                // Handle the case where no questions were selected
                TempData["errorMsg"] = "No questions selected.";
                return RedirectToAction("GetExistingQuestions", "Question", new { mcqId, jobTitleId, jobLevelId }); 
            }

            // Add selected questions to McqQuestion entity
            foreach (var questionId in selectedQuestionIds)
            {
                // Check if the McqQuestion already exists for the selected question and MCQ
                var existingMcqQuestion = await _mcqQuestionRepository.GetMcqQuestionByIdAsync(mcqId, questionId);

                if (existingMcqQuestion == null)
                {
                    // If the McqQuestion doesn't exist, add it
                    var mcqQuestion = new McqQuestion
                    {
                        McqId = mcqId,
                        QuestionId = questionId
                    };

                    await _mcqQuestionRepository.AddMcqQuestionAsync(mcqQuestion);
                }

            }

            TempData["successMsg"] = "Selected questions saved successfully.";

            return RedirectToAction("QuestionList", new { mcqId, jobTitleId, jobLevelId }); 
        }

        [Authorize(Policy = "RequireAdminOrEditorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int mcqId, int questionId)
        {
            if (mcqId <= 0 || questionId <= 0)
            {
                TempData["errorMsg"] = "Invalid parameters. An MCQ question should be selected";
                return RedirectToAction("QuestionList");
            }
            var deletedMcqQuestion = await _mcqQuestionRepository.DeleteMcqQuestionAsync(mcqId, questionId);

            if (deletedMcqQuestion != null)
            {
                TempData["successMsg"] = "Question deleted successfully.";
            }
            else
            {
                TempData["errorMsg"] = "Question not found or unable to delete.";
            }

            var mcq = await _mCQRepository.GetMCQByIdAsync(mcqId);
            var jobTitleId = (mcq != null) ? mcq.JobTitleId : 0;
            var jobLevelId = (mcq != null) ? mcq.JobLevel : 0;

            return RedirectToAction("QuestionList", new { mcqId, jobTitleId, jobLevelId });
        }
    }



}

