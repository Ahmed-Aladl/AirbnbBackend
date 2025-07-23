using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.HostReply
{
    public class HostReviewEditReplyDTO
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ReviewId { get; set; }
        public string Comment { get; set; }

    }
}
