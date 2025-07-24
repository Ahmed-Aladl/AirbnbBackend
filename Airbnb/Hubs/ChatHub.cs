using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace Airbnb.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        public async Task JoinChat(string chatSessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatSessionId);
        }

        public async Task LeaveChat(string chatSessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatSessionId);
        }
    }
}
