using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums.Chat;

namespace Application.DTOs.Chat.Requests
{
    public class SendMessageRequest
    {
        public string ChatSessionId { get; set; }
        public string MessageText { get; set; }
        public MessageType MessageType { get; set; } = MessageType.Text;
    }

}
