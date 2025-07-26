using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Chat
{
    public class MessageReaction
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();


        [Required]
        [MaxLength(20)]
        public string ReactionType { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property

        [ForeignKey(nameof(User))]
        [Required]
        public string UserId { get; set; }
        public User User{ get; set; } = null!;


        [ForeignKey(nameof(Message))]
        [Required]
        public string MessageId { get; set; }
        public Message Message { get; set; } = null!;
    }

}
