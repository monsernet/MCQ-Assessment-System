using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Repositories.Options;
using MCQInterviews.Repositories.Questions;
using OfficeOpenXml;

namespace MCQInterviews.Repositories.ExcelImport
{
    public class QuestionExcelImport : IQuestionExcelImport
    {
        private readonly IQuestionRepository questionRepository;
        private readonly IOptionRepository optionRepository;

        public QuestionExcelImport(
            IQuestionRepository questionRepository,
            IOptionRepository optionRepository
            )
        {
            this.questionRepository = questionRepository;
            this.optionRepository = optionRepository;
        }

        public async Task<List<BulkQuestionExcelViewModel>> ReadQuestionsFromExcel(Stream excelFileStream, int jobTitleId, int jobLevelId, int difficultyTypeId)
        {
            var bulkQuestions = new List<BulkQuestionExcelViewModel>();

            using (var package = new ExcelPackage(excelFileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];

                //Question in column 1
                int textColumnIndex = 1;
                // Options start from column 2
                int optionsStartColumnIndex = 2;
                //The index of corrected option is in the last column
                int correctOptionColumnIndex = worksheet.Dimension.End.Column;

                // Start uploading the data from Row 2 -- keep row 1 for headers
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    var bulkQuestion = new BulkQuestionExcelViewModel
                    {
                        Text = worksheet.Cells[row, textColumnIndex].Text,
                        CorrectOptionIndex = int.Parse(worksheet.Cells[row, correctOptionColumnIndex].Text)
                    };

                    //Retreive data from the sheet cells
                    for (int col = optionsStartColumnIndex; col < correctOptionColumnIndex; col++)
                    {
                        bulkQuestion.BulkOptions.Add(worksheet.Cells[row, col].Text);
                    }

                    bulkQuestions.Add(bulkQuestion);
                }
            }

            // Convert BulkQuestionExcelViewModel to List<Question>
            var questionsEntities = await ConvertToQuestionEntities(bulkQuestions, jobTitleId, jobLevelId, difficultyTypeId);



            return bulkQuestions;
        }

        private async Task<List<Question>> ConvertToQuestionEntities(List<BulkQuestionExcelViewModel> bulkQuestions, int jobTitleId, int jobLevelId, int difficultyTypeId)
        {
            var questionsEntities = new List<Question>();

            foreach (var bulkQuestion in bulkQuestions)
            {
                var questionEntity = new Question
                {
                    Text = bulkQuestion.Text,
                    JobTitleId = jobTitleId,
                    QuestionDifficultyTypeId = difficultyTypeId
                };
                var quest = await questionRepository.AddQuestionAsync(questionEntity);
                var questId = quest.Id;
                int countOption = 0;
                bool isCorrectAnswer = false;

                // Add options to the question entity
                foreach (var optionText in bulkQuestion.BulkOptions)
                {
                    countOption++;
                    isCorrectAnswer = (bulkQuestion.CorrectOptionIndex == countOption) ? true : false;
                    var optionEntity = new QuestionOption
                    {
                        Text = optionText,
                        IsCorrect = isCorrectAnswer,
                        QuestionId = questId
                    };
                    await optionRepository.AddOptionAsync(questId, optionEntity);
                }


            }

            return questionsEntities;
        }

        private async Task SaveQuestionsAndOptionsToDatabase(List<Question> questions)
        {
            foreach (var question in questions)
            {
                // Save the question to get its Id
                var quest = await questionRepository.AddQuestionAsync(question);
                var questId = quest.Id;

                // Save options related to the question
                /* foreach (var option in question.QuestionOptions)
                {
                    var opt = new QuestionOption
                    {
                        QuestionId = questId,
                        Text = option.Text,
                        IsCorrect = option.IsCorrect
                    };
                    // Save the option to the database
                    await optionRepository.AddOptionAsync(questId, opt);
                }*/
            }
        }
    }
}
