using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.HostReply
{
    public class HostReviewReplyDto
    {
        
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string HostId { get; set; }
        public int ReviewId { get; set; }
     
        
    }
}
