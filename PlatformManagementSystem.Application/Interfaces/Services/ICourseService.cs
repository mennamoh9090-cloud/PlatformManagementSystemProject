using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlatformManagementSystem.Application.DTOs;
using PlatformManagementSystem.Application.DTOs.Course;

namespace PlatformManagementSystem.Application.Interfaces.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllAsync();
        Task<CourseDto> GetByIdAsync(int id);
        Task CreateAsync(CreateCourseDto dto, string instructorId);
        Task ApproveCourseAsync(int courseId);
    }
}

