using DevFastTrack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DevFastTrack.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<CorporateTrainingRequest> CorporateTrainingRequests => Set<CorporateTrainingRequest>();
    public DbSet<CorporateEmployee> CorporateEmployees => Set<CorporateEmployee>();
    public DbSet<CorporateInvoice> CorporateInvoices => Set<CorporateInvoice>();
    public DbSet<TrainerProfile> TrainerProfiles => Set<TrainerProfile>();
    public DbSet<CorporateCompany> CorporateCompanies => Set<CorporateCompany>();
    
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<AssessmentAttempt> AssessmentAttempts => Set<AssessmentAttempt>();
    public DbSet<AttemptAnswer> AttemptAnswers => Set<AttemptAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Course
        modelBuilder.Entity<Course>()
            .Property(c => c.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Course>()
            .Property(c => c.OriginalPrice)
            .HasColumnType("decimal(18,2)");

        // Enrollment
        modelBuilder.Entity<Enrollment>()
            .Property(e => e.AmountPaid)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // CorporateInvoice
        modelBuilder.Entity<CorporateInvoice>()
            .Property(i => i.Amount)
            .HasColumnType("decimal(18,2)");

        // TrainerProfile
        modelBuilder.Entity<TrainerProfile>()
            .Property(t => t.MinSalary)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<TrainerProfile>()
            .Property(t => t.MaxSalary)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<TrainerProfile>()
            .HasIndex(t => t.Email)
            .IsUnique();

        // CorporateCompany
        modelBuilder.Entity<CorporateCompany>()
            .HasIndex(c => c.Email)
            .IsUnique();

        // Assessment
        modelBuilder.Entity<Assessment>()
            .HasOne(a => a.Course)
            .WithMany(c => c.Assessments)
            .HasForeignKey(a => a.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Assessment>()
            .Property(a => a.TotalMarks)
            .HasColumnType("decimal(18,2)");

        // Question
        modelBuilder.Entity<Question>()
            .Property(q => q.Marks)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Assessment)
            .WithMany(a => a.Questions)
            .HasForeignKey(q => q.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // QuestionOption
        modelBuilder.Entity<QuestionOption>()
            .HasOne(o => o.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // AssessmentAttempt
        modelBuilder.Entity<AssessmentAttempt>()
            .Property(a => a.Score)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<AssessmentAttempt>()
            .HasOne(a => a.Assessment)
            .WithMany(ass => ass.Attempts)
            .HasForeignKey(a => a.AssessmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AssessmentAttempt>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // AttemptAnswer
        modelBuilder.Entity<AttemptAnswer>()
            .Property(a => a.MarksObtained)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<AttemptAnswer>()
            .HasOne(a => a.AssessmentAttempt)
            .WithMany(att => att.Answers)
            .HasForeignKey(a => a.AssessmentAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AttemptAnswer>()
            .HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
