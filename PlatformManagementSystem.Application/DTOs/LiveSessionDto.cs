using PlatformManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformManagementSystem.Application.DTOs
{
    public class LiveSessionDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = "";
        public DateTime StartTime { get; set; }
        public bool IsActive { get; set; }
        public string MeetingUrl { get; set; } = "";
    }
}
