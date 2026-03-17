using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities;

public class LiveSessionAttendance : BaseEntity
{
    public int LiveSessionId { get; set; }
    public LiveSession LiveSession { get; set; } = null!;

    public string StudentId { get; set; } = null!;
    public ApplicationUser Student { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
