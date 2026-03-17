using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities
{
    public class WhiteboardEvent : BaseEntity
    {
        public int Id { get; set; }
        public int LiveSessionId { get; set; }
        public LiveSession LiveSession { get; set; }
        public string UserId { get; set; } = "";

        public string EventType { get; set; } = "";

        public string Data { get; set; } = "";
        public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
