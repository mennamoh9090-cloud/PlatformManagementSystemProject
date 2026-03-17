using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities
{
    public class StudentLessonProgress : BaseEntity
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = "";  
        public ApplicationUser Student { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

