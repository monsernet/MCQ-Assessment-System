namespace MCQInterviews.Models.ViewModels
{
    public class BulkQuestionExcelViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<string> BulkOptions { get; set; } = new List<string>();
        public int CorrectOptionIndex { get; set; }
    }
}
