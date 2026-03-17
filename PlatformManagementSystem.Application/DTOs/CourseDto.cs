using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformManagementSystem.Application.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public decimal Price { get; set; }

        public string InstructorName { get; set; } = "";

        public int CategoryId { get; set; }
    }
}
