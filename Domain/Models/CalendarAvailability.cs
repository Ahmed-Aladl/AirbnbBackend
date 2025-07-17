using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public   class CalendarAvailability
    
    {
        public int CalendarID { get; set; }


        //[ForeignKey(nameof(Properties))]
        public int PropertyID { get; set; } 

        public DateTime date { get; set; }   

        public bool IsAvailable { get; set; } 

        public decimal price { get; set; }


    }
}
