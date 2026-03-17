using PlatformManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformManagementSystem.Domain.Entities;

public class WhiteboardElement : BaseEntity
{
    public string SessionId { get; set; } = default!;

    public string Type { get; set; } = default!;
    // rectangle, circle, line, arrow, freehand, text

    public string Data { get; set; } = default!;
    // JSON data (coordinates, size, text, etc)

    public string Color { get; set; } = "#000000";

    public string CreatedByUserId { get; set; } = default!;
}

