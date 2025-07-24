using Application.DTOs.Chat.MessageDtos;
using Application.Interfaces.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Airbnb.Hubs
{
    public class ChatNotifier : IChatNotifier
    {
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;

        public ChatNotifier(IHubContext<ChatHub, IChatClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyMessageSentAsync(string chatSessionId, MessageDto message)
        {
            return _hubContext.Clients.Group(chatSessionId).ReceiveMessage(message);
        }
    }
}
