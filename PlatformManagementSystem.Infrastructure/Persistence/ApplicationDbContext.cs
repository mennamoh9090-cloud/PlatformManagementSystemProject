using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlatformManagementSystem.Domain.Entities;

namespace PlatformManagementSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Core Tables 

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<StudentLessonProgress> StudentLessonProgresses { get; set; }
        public DbSet<Review> Reviews { get; set; }

        //  Exams 

        public DbSet<Exam> Exams { get; set; }  
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<StudentExam> StudentExams { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }

        //  Live 

        public DbSet<LiveSession> LiveSessions { get; set; }
        public DbSet<LiveSessionAttendance> LiveSessionAttendances { get; set; }
        public DbSet<WhiteboardEvent> WhiteboardEvents { get; set; }
        public DbSet<WhiteboardElement> WhiteboardElements { get; set; }
        public DbSet<RaiseHand> RaiseHands { get; set; }
        public DbSet<SessionAttendance> SessionAttendances { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Course>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            builder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();

            builder.Entity<StudentLessonProgress>()
                .HasIndex(p => new { p.StudentId, p.LessonId })
                .IsUnique();

            builder.Entity<Review>()
                .HasIndex(r => new { r.StudentId, r.CourseId })
                .IsUnique();

            // Exam Relations 

            builder.Entity<Course>()
                .HasMany(c => c.Exams)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Question>()
                .HasOne(q => q.Exam)
                .WithMany(e => e.Questions)
                .HasForeignKey(q => q.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentExam>()
                .HasOne(se => se.Exam)
                .WithMany(e => e.StudentExams)
                .HasForeignKey(se => se.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentAnswer>()
                .HasOne(sa => sa.StudentExam)
                .WithMany(se => se.StudentAnswers)
                .HasForeignKey(sa => sa.StudentExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Question)
                .WithMany(q => q.StudentAnswers)
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enrollment 

            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Lesson 

            builder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            //  StudentLessonProgress 

            builder.Entity<StudentLessonProgress>()
                .HasOne(p => p.Course)
                .WithMany()
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StudentLessonProgress>()
                .HasOne(p => p.Lesson)
                .WithMany(l => l.ProgressRecords)
                .HasForeignKey(p => p.LessonId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StudentLessonProgress>()
                .HasOne(p => p.Student)
                .WithMany()
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            // Live 

            builder.Entity<LiveSession>()
                .HasOne(ls => ls.Instructor)
                .WithMany()
                .HasForeignKey(ls => ls.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<LiveSessionAttendance>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<WhiteboardEvent>()
                .HasOne(w => w.LiveSession)
                .WithMany()
                .HasForeignKey(w => w.LiveSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}