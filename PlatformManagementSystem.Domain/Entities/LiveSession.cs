using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities;

public class LiveSession : BaseEntity
{
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public string InstructorId { get; set; } = null!;
    public ApplicationUser Instructor { get; set; } = null!;

    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public bool IsActive { get; set; } = true;
    public string MeetingUrl { get; set; } = "";

    public ICollection<LiveSessionAttendance> Attendances { get; set; } = new List<LiveSessionAttendance>();
    public string Title { get; set; } = "";
}

