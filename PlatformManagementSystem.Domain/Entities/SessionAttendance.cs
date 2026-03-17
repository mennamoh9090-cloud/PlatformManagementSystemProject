using PlatformManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformManagementSystem.Domain.Entities;

public class SessionAttendance : BaseEntity
{
    public string SessionId { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
