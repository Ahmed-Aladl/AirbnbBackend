using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Chat
{
    public class Message
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string ChatSessionId { get; set; }

        [Required]
        public string SenderId { get; set; }
        public bool IsHost { get; set; }

        public string? MessageText { get; set; }

        [MaxLength(50)]
        public string MessageType { get; set; } = "text";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsEdited { get; set; } = false;

        public DateTime? EditedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation property
        public ChatSession ChatSession { get; set; } = null!;

        public ICollection<MessageReaction> Reactions { get; set; }
        public ICollection<MessageReadStatus> ReadStatuses { get; set; }

        public ReservationRequest ReservationRequest { get; set; }
    }

}
