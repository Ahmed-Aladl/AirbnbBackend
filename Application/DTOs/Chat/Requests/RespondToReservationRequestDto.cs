using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.ChatSessionDtos;
using Application.DTOs.Chat.MessageDtos;
using Application.DTOs.Chat.ReservationRequestDtos;
using Application.DTOs.PropertyDTOS;
using Domain.Enums.Chat;
using Domain.Models.Chat;

namespace Application.DTOs.Chat.Requests
{
    public class RespondToReservationRequestDto
    {
        public ReservationRequestDto LatestReservationRequest { get; set; }
        public ChatSessionDto? ChatSession { get; set; }
        public PropertyDisplayDTO Proeprty {  get; set; }
        public List<MessageDto>? Messages { get; set; }
        
    }
}
