using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace PlatformManagementSystem.API.Hubs
{
    [Authorize]
    public class LiveBoardHub : Hub
    {
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
            var groupName = $"session-{sessionId}";

            await Clients.Group(groupName)
                .SendAsync("ReceiveDraw", drawData);
        }

       


    }
}
