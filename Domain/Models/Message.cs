using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public  class Message
    {
        public int MessageID { get; set; }

        //[ForeignKey(nameof())]
        public string SenderID  { get; set; }

        //[ForeignKey(nameof())]
        public string ReceiverID { get; set; }


        //[ForeignKey(nameof())
        public int PropertyID { get; set; }
        public string MesageText { get; set; } 

        public DateTime SentAt { get; set; }
        public bool Isread { get; set; }


    }
}
