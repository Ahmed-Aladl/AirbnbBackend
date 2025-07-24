using System.Text.RegularExpressions;
using Application.DTOs.Chat.MessageDtos;
using Microsoft.AspNetCore.SignalR;

namespace Airbnb.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(MessageDto message);
    }

    
}
