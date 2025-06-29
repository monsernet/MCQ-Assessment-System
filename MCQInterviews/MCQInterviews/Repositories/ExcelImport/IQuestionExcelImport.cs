using MCQInterviews.Models.ViewModels;

namespace MCQInterviews.Repositories.ExcelImport
{
    public interface IQuestionExcelImport
    {
        public Task<List<BulkQuestionExcelViewModel>> ReadQuestionsFromExcel(Stream excelFileStream, int jobTitleId, int jobLevelId, int difficultyTypeId);
    }
}
