using PlatformManagementSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformManagementSystem.Application.Interfaces.Services
{
    public interface ILiveSessionService
    {
        Task<int> CreateAsync(int courseId, string title, DateTime startTime, string meetingUrl);
        Task<List<LiveSessionDto>> GetByCourseAsync(int courseId);
        Task StartSessionAsync(int sessionId);
        Task EndSessionAsync(int sessionId);
        Task<int> CreateSessionAsync(string instructorId, int courseId, string title, DateTime startTime);
    }

}
