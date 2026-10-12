using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Domain.Entities;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlatformManagementSystem.API.Hubs
{
    [Authorize]
    public class LiveBoardHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public LiveBoardHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task JoinSessionGroup(int sessionId)
        {
            var groupName = $"session-{sessionId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // Fetch history of whiteboard strokes for this session and send to caller
            var history = (await _unitOfWork.WhiteboardEvents
                .FindAsync(w => w.LiveSessionId == sessionId))
                .OrderBy(w => w.CreatedAt)
                .Select(w => new
                {
                    w.Id,
                    w.EventType,
                    w.Data,
                    w.CreatedAt
                })
                .ToList();

            if (history.Any())
            {
                await Clients.Caller.SendAsync("ReceiveHistory", history);
            }
        }

        public async Task LeaveSessionGroup(int sessionId)
        {
            var groupName = $"session-{sessionId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessage(int sessionId, string message)
        {
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value
                           ?? Context.User?.FindFirst(ClaimTypes.Email)?.Value
                           ?? "User";

            var groupName = $"session-{sessionId}";

            await Clients.Group(groupName)
                .SendAsync("ReceiveMessage", userName, message);
        }

        public async Task Draw(int sessionId, object drawData)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var groupName = $"session-{sessionId}";

            // Broadcast to other participants in session
            await Clients.OthersInGroup(groupName)
                .SendAsync("ReceiveDraw", drawData);

            // Persist draw event
            try
            {
                var json = JsonSerializer.Serialize(drawData);
                string eventType = "draw";

                if (drawData is JsonElement jsonElement && jsonElement.TryGetProperty("type", out var typeProp))
                {
                    eventType = typeProp.GetString() ?? "draw";
                }

                if (eventType == "clear")
                {
                    var oldEvents = (await _unitOfWork.WhiteboardEvents
                        .FindAsync(w => w.LiveSessionId == sessionId)).ToList();

                    foreach (var evt in oldEvents)
                    {
                        _unitOfWork.WhiteboardEvents.Remove(evt);
                    }
                }
                else
                {
                    var whiteboardEvent = new WhiteboardEvent
                    {
                        LiveSessionId = sessionId,
                        UserId = userId,
                        EventType = eventType,
                        Data = json,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.WhiteboardEvents.AddAsync(whiteboardEvent);
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                // Persistence fail-safe: realtime broadcast already succeeded
            }
        }
    }
}
