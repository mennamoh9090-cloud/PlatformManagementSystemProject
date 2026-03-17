using PlatformManagementSystem.Application.DTOs;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Application.Interfaces.Services;
using PlatformManagementSystem.Domain.Entities;

namespace PlatformManagementSystem.Application.Services
{
    public class LiveSessionService : ILiveSessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LiveSessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateAsync(int courseId, string title, DateTime startTime, string meetingUrl)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception("Course not found");

            var session = new Domain.Entities.LiveSession
            {
                CourseId = courseId,
                Title = title,
                StartTime = startTime.ToUniversalTime(),
                MeetingUrl = meetingUrl,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                InstructorId = course.InstructorId
            };

            await _unitOfWork.LiveSessions.AddAsync(session);
            await _unitOfWork.SaveChangesAsync();

            return session.Id;
        }

        public async Task<int> CreateSessionAsync(
            string instructorId,
            int courseId,
            string title,
            DateTime startTime)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);

            if (course == null)
                throw new Exception("Course not found");

            if (course.InstructorId != instructorId)
                throw new Exception("Unauthorized");

            var session = new LiveSession
            {
                CourseId = courseId,
                InstructorId = instructorId,
                Title = title,
                StartTime = startTime.ToUniversalTime(),
                IsActive = false,
                MeetingUrl = string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.LiveSessions.AddAsync(session);
            await _unitOfWork.SaveChangesAsync();

            return session.Id;
        }


        public async Task EndSessionAsync(int sessionId)
        {
            var session = await _unitOfWork.LiveSessions.GetByIdAsync(sessionId);

            if (session == null)
                throw new Exception("Session not found");

            session.IsActive = false;
            session.EndTime = DateTime.UtcNow;

            _unitOfWork.LiveSessions.Update(session);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<LiveSessionDto>> GetByCourseAsync(int courseId)
        {
            var sessions = await _unitOfWork.LiveSessions
                .FindAsync(s => s.CourseId == courseId);

            return sessions
                .OrderByDescending(s => s.StartTime)
                .Select(s => new LiveSessionDto
                {
                    Id = s.Id,
                    CourseId = s.CourseId,
                    Title = s.Title,
                    StartTime = s.StartTime,
                    IsActive = s.IsActive,
                    MeetingUrl = s.MeetingUrl ?? string.Empty
                })
                .ToList();
        }

        public async Task JoinSessionAsync(int sessionId, string studentId)
        {
            var session = await _unitOfWork.LiveSessions.GetByIdAsync(sessionId);

            if (session == null || !session.IsActive)
                throw new Exception("Session not active");

            var exists = await _unitOfWork.LiveSessionAttendances
                .FindAsync(x =>
                    x.LiveSessionId == sessionId &&
                    x.StudentId == studentId);

            if (!exists.Any())
            {
                var attendance = new LiveSessionAttendance
                {
                    LiveSessionId = sessionId,
                    StudentId = studentId,
                    JoinedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.LiveSessionAttendances.AddAsync(attendance);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task StartSessionAsync(int sessionId)
        {
            var session = await _unitOfWork.LiveSessions.GetByIdAsync(sessionId);
            if (session == null)
                throw new Exception("Session not found");

            session.IsActive = true;
            if (session.StartTime == default)
                session.StartTime = DateTime.UtcNow;

            _unitOfWork.LiveSessions.Update(session);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

