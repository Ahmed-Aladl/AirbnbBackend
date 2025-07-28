using Application.DTOs.Chat.MessageDtos;
using Application.Interfaces.Hubs;
using Application.Interfaces.Services;
using Domain.Models;
using Microsoft.AspNetCore.SignalR;

namespace Airbnb.Hubs
{
    public class ChatNotifier : IChatNotifier
    {
        private readonly IUserConnectionService _userConnections;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatNotifier(IUserConnectionService userConnections, IHubContext<ChatHub> hubContext)
        {
            _userConnections = userConnections;
            _hubContext = hubContext;
        }


        public async Task NotifyMessageSentAsync(string userId, MessageDto message)
        {
            Console.WriteLine($"*************************************\n\n\n\n\n\nfrom notifier*************************************\n{message?.MessageText}\n\n\\n\n\n");
            var connections = _userConnections.GetConnections(userId);
            foreach (var connectionId in connections)
            {
                await _hubContext.Clients.Client(connectionId)
                    .SendAsync(nameof(IChatClient.ReceiveMessage), message);
            }


        }
    }
}
