namespace MCQInterviews.Models.Domain
{
    public class Theme
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<JobTitle> JobTitles { get; set; }
    }

}
