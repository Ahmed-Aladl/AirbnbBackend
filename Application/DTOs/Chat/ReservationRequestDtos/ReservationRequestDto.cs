using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums.Chat;

namespace Application.DTOs.Chat.ReservationRequestDtos
{
    public class ReservationRequestDto
    {
        public string Id { get; set; }
        public string ChatSessionId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int GuestCount { get; set; }
        public decimal TotalAmount { get; set; }
        public ReservationRequestStatus RequestStatus { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string ResponseMessage { get; set; }
        public int NightCount { get; set; }
        public decimal PricePerNight { get; set; }
    }

}
