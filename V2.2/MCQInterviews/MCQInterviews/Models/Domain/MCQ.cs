using static MCQInterviews.Enums.Enums;

namespace MCQInterviews.Models.Domain
{
    public class MCQ
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int NbQuestions { get; set; }
        public MCQType MCQType { get; set; }


        public int JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }
        public int JobLevel { get; set; }

        // Foreign key for DifficultyType
        public int McqDifficultyTypeId { get; set; } = 1; // Default value set to 1 -- "Easy type"
        public McqDifficultyType McqDifficultyType { get; set; }



        // Navigation properties
        public ICollection<Question> Questions { get; set; }
        public ICollection<MCQTestResult> TestResults { get; set; }

    }

}
