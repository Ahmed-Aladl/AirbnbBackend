using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.MessageDtos;

namespace Application.Interfaces.Hubs
{
    public interface IChatNotifier
    {
        Task NotifyMessageSentAsync(string chatSessionId, MessageDto message);
    }
}
