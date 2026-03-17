using PlatformManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformManagementSystem.Domain.Entities;

public class RaiseHand : BaseEntity
{
    public string SessionId { get; set; } = default!;
    public string StudentId { get; set; } = default!;
    public DateTime RaisedAt { get; set; } = DateTime.UtcNow;
}
