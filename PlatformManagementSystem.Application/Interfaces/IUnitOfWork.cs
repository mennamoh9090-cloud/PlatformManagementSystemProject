using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PlatformManagementSystem.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Course> Courses { get; }
    IGenericRepository<Enrollment> Enrollments { get; }
    IGenericRepository<Category> Categories { get; }

    IGenericRepository<ApplicationUser> Users { get; }

    IGenericRepository<LiveSession> LiveSessions { get; }
    IGenericRepository<LiveSessionAttendance> LiveSessionAttendances { get; }
    IGenericRepository<Exam> Exams { get; }
    IGenericRepository<Question> Questions { get; }
    IGenericRepository<Answer> Answers { get; }
    IGenericRepository<StudentExam> StudentExams { get; }
    IGenericRepository<StudentAnswer> StudentAnswers { get; }
    IGenericRepository<StudentLessonProgress> StudentLessonProgresses { get; }
    IGenericRepository<WhiteboardEvent> WhiteboardEvents { get; }

    IGenericRepository<Lesson> Lessons { get; }

    Task<int> SaveChangesAsync();
}
