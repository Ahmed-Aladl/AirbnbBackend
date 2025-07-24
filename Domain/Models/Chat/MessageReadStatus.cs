using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Chat
{
    public class MessageReadStatus
    {
        //key
        public string MessageId { get; set; }
        //key
        public string UserId { get; set; }

        public DateTime ReadAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Message Message { get; set; } = null!;
    }

}
