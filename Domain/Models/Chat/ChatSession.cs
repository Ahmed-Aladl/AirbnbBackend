using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Chat
{
    public class ChatSession
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string? LastMessageText { get; set; }

        public DateTime? LastMessageAt { get; set; }

        public int UnreadCountForHost { get; set; } = 0;

        public int UnreadCountForUser { get; set; } = 0;

        [ForeignKey(nameof(Property))]
        [Required]
        public int PropertyId { get; set; }

        [ForeignKey(nameof(User))]
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(Host))]
        [Required]
        public string HostId { get; set; }


        // Navigation property
        public Property Property { get; set; } = null!;
        public User User { get; set; } = null!;
        public User Host{ get; set; } = null!;

        public ICollection<Message>? Messages { get; set; }
        // Optional: composite unique constraint via Fluent API
    }

}
