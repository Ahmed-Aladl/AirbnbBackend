using System.Text.RegularExpressions;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace Airbnb.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IUserConnectionService _userConnections;
        private readonly IConfiguration configuration;

        public ChatHub(
                IUserConnectionService userConnections,
                IConfiguration configuration
            )
        {
            _userConnections = userConnections;
            this.configuration = configuration;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier; // Use Context.User.Identity.Name or claim
            userId = userId ?? configuration["userId"];
            if(userId != null ) 
                _userConnections.AddConnection(userId, Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task JoinChat(string chatSessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatSessionId);
        }

        public async Task LeaveChat(string chatSessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatSessionId);
        }


        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context?.UserIdentifier;
                
            if(userId !=null)
                _userConnections.RemoveConnection(userId, Context?.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
        
    }
}
