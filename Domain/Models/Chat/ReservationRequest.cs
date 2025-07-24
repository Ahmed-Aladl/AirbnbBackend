using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Chat
{
    public class ReservationRequest
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();


        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public int GuestCount { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(20)]
        public string RequestStatus { get; set; } = "pending";

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RespondedAt { get; set; }

        [MaxLength(500)]
        public string? ResponseMessage { get; set; }



        // Navigation properties
        [ForeignKey(nameof(ChatSession))]
        [Required]
        public string ChatSessionId { get; set; }
        public ChatSession ChatSession { get; set; } = null!;

        [ForeignKey(nameof(Message))]
        public string? MessageId { get; set; }
        public Message? Message { get; set; }
    }

}
