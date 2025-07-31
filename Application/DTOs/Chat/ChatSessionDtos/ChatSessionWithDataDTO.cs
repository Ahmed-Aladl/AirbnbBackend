using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Chat;

namespace Application.DTOs.Chat.ChatSessionDtos
{
    public class ChatSessionWithDataDTO
    {
        public ChatSession ChatSession { get; set; }
        public string ProfilePictureURL {  get; set; }
    }
}
