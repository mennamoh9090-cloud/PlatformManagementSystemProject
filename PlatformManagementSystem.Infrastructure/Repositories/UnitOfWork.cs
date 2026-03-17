using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Domain.Entities;
using PlatformManagementSystem.Infrastructure.Persistence;

namespace PlatformManagementSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IGenericRepository<Course> Courses { get; }
    public IGenericRepository<Enrollment> Enrollments { get; }
    public IGenericRepository<Category> Categories { get; }

    public IGenericRepository<ApplicationUser> Users { get; }

    public IGenericRepository<LiveSession> LiveSessions { get; }
    public IGenericRepository<LiveSessionAttendance> LiveSessionAttendances { get; }

    public IGenericRepository<Exam> Exams { get; }
    public IGenericRepository<Question> Questions { get; }
    public IGenericRepository<Answer> Answers { get; }
    public IGenericRepository<StudentExam> StudentExams { get; private set; }

    public IGenericRepository<StudentAnswer> StudentAnswers { get; }
    public IGenericRepository<StudentLessonProgress> StudentLessonProgresses { get; }

    public IGenericRepository<WhiteboardEvent> WhiteboardEvents { get; }

    public IGenericRepository<Lesson> Lessons { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        Courses = new GenericRepository<Course>(_context);
        Enrollments = new GenericRepository<Enrollment>(_context);
        Categories = new GenericRepository<Category>(_context);
        Exams = new GenericRepository<Exam>(_context);
        Questions = new GenericRepository<Question>(_context);
        Answers = new GenericRepository<Answer>(_context);
        StudentExams = new GenericRepository<StudentExam>(_context);
        StudentAnswers = new GenericRepository<StudentAnswer>(_context);
        Users = new GenericRepository<ApplicationUser>(_context);
        LiveSessions = new GenericRepository<LiveSession>(_context);
        LiveSessionAttendances = new GenericRepository<LiveSessionAttendance>(_context);
        StudentLessonProgresses = new GenericRepository<StudentLessonProgress>(_context);
        WhiteboardEvents = new GenericRepository<WhiteboardEvent>(_context);
        Lessons = new GenericRepository<Lesson>(_context);

    }


    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    
}

