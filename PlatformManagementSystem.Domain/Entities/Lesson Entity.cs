using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities
{
    public class Lesson : BaseEntity
    {
        public string Title { get; set; } = "";

        public string Content { get; set; } = "";

        public int CourseId { get; set; }
        public Course Course { get; set; }  

        public ICollection<StudentLessonProgress> ProgressRecords { get; set; }
    }
}
