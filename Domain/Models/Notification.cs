using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public  class Notification
    {
        public int NotificationID { get; set; }

        //[ForeignKey(nameof())]

        public string UserID  { get; set; } 

        public string Message { get; set; } 

        public bool isRead { get; set; } 

        public DateTime CreatedAt { get; set; }


    }
}
