using AuthSystem.Areas.Identity.Data;
using MCQInterviews.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MCQInterviews.Data
{
    public class ApplicaationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicaationDbContext(DbContextOptions<ApplicaationDbContext> options) : base(options)
        {

        }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<JobLevel> JobLevels { get; set; }
        public DbSet<MCQ> MCQs { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<ResponseType> ResponseTypes { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<McqQuestion> McqQuestions { get; set; }
        public DbSet<MCQTestResult> MCQTestResults { get; set; }
        public DbSet<QuestionDifficultyType> QuestionDifficultyTypes { get; set; }
        public DbSet<McqDifficultyType> McqDifficultyTypes { get; set; }
        public DbSet<DifficultyAllocation> DifficultyAllocations { get; set; }

        public DbSet<VoiceSample> VoiceSamples { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<MCQTestResult>()
            .HasOne(tr => tr.User)
            .WithMany(u => u.TestResults)
            .HasForeignKey(tr => tr.UserId);

            builder.Entity<MCQTestResult>()
                .HasOne(tr => tr.MCQ)
                .WithMany(mcq => mcq.TestResults)
                .HasForeignKey(tr => tr.MCQId);
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.RegistrationDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()"); // Set default value to current UTC date and time
            });
            builder.Entity<VoiceSample>()
            .HasOne(v => v.Candidate)
            .WithMany() 
            .HasForeignKey(v => v.CandidateId)
            .OnDelete(DeleteBehavior.Restrict); // prevents deletion of user if voice samples exist


            base.OnModelCreating(builder);
        }



    }
}
