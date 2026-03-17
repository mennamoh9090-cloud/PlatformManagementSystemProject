using PlatformManagementSystem.Application.DTOs;
using PlatformManagementSystem.Application.DTOs.Course;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Application.Interfaces.Services;
using PlatformManagementSystem.Domain.Entities;
using PlatformManagementSystem.Domain.Enums;

namespace PlatformManagementSystem.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            var courses = await _unitOfWork.Courses.GetAllAsync();

            return courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Price = c.Price,
                CategoryId = c.CategoryId
            });
        }

       
        public async Task<CourseDto?> GetByIdAsync(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);

            if (course == null)
                return null;

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                CategoryId = course.CategoryId
            };
        }

        public async Task<CourseDto> CreateAsync(CreateCourseDto dto, string instructorId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);

            if (category == null)
                throw new Exception("Category not found");

            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                InstructorId = instructorId,
                Status = CourseStatus.Pending,   
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                CategoryId = course.CategoryId
            };
        }

        public async Task ApproveCourseAsync(int courseId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);

            if (course == null)
                throw new Exception("Course not found");

            course.Status = CourseStatus.Approved;

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);

            if (course == null)
                throw new Exception("Course not found");

            _unitOfWork.Courses.Remove(course);
            await _unitOfWork.SaveChangesAsync();
        }

        Task ICourseService.CreateAsync(CreateCourseDto dto, string instructorId)
        {
            return CreateAsync(dto, instructorId);
        }
    }
}

