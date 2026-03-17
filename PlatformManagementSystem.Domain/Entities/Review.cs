using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities
{
    public class Review : BaseEntity
    {
        public string StudentId { get; set; } = "";
        public required ApplicationUser Student { get; set; }

        public int CourseId { get; set; }
        public required Course Course { get; set; }

        public int Rating { get; set; }

        public required string Comment { get; set; }
    }
}
