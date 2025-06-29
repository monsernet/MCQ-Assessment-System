namespace MCQInterviews.Models.Domain
{
    public class JobTitle
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public int ThemeId { get; set; }
        public Theme Theme { get; set; }


    }

}
