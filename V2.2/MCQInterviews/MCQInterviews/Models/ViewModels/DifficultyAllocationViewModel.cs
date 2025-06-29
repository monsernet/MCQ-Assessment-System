namespace MCQInterviews.Models.ViewModels
{
    public class DifficultyAllocationViewModel
    {
        public int Id { get; set; }
        public double Percentage { get; set; }
        public int NbQuestions { get; set; }
        public int McqDifficultyTypeId { get; set; }
        public string McqDifficultyTypeName { get; set; }
        public int QuestionDifficultyTypeId { get; set; }
        public string QuestionDifficultyTypeName { get; set; }
        public int CurrentQuestionCount { get; set; }
    }
}
