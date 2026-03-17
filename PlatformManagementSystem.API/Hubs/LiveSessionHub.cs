using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PlatformManagementSystem.API.Hubs;

[Authorize]
public class LiveSessionHub : Hub
{
    public async Task JoinCourseGroup(int courseId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Course_{courseId}");
    }
}