using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums.Chat;

namespace Application.DTOs.Chat.Requests
{
    public class RespondToReservationRequestDto
    {
        public string Id { get; set; }
        public ReservationRequestStatus Status { get; set; }
        public string ResponseMessage { get; set; }
    }
}
