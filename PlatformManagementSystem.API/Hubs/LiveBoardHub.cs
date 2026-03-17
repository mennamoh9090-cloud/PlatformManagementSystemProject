using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Domain.Entities;
using System.Security.Claims;
using System.Text.Json;

namespace PlatformManagementSystem.API.Hubs
{
    [Authorize]
    public class LiveBoardHub : Hub
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public LiveBoardHub(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task JoinSessionGroup(int sessionId)
        {
            var groupName = $"session-{sessionId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveSessionGroup(int sessionId)
        {
            var groupName = $"session-{sessionId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessage(int sessionId, string message)
        {
            var userName = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            var groupName = $"session-{sessionId}";
            await Clients.Group(groupName)
                .SendAsync("ReceiveMessage", userName, message);
        }

        public async Task Draw(int sessionId, object drawData)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var groupName = $"session-{sessionId}";

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var session = await unitOfWork.LiveSessions.GetByIdAsync(sessionId);
                    if (session != null)
                    {
                        var dataJson = drawData is JsonElement je ? je.GetRawText() : JsonSerializer.Serialize(drawData);
                        var ev = new WhiteboardEvent
                        {
                            LiveSessionId = sessionId,
                            UserId = userId,
                            EventType = "Draw",
                            Data = dataJson,
                            CreatedAt = DateTime.UtcNow
                        };
                        await unitOfWork.WhiteboardEvents.AddAsync(ev);
                        await unitOfWork.SaveChangesAsync();
                    }
                }
            }
            catch
            {
                // still broadcast even if save fails
            }

            await Clients.OthersInGroup(groupName)
                .SendAsync("ReceiveDraw", drawData);
        }
    }
}
