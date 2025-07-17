using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models
{
    public   class CalendarAvailability
    
    {
        public int Id{ get; set; }


        
        public DateTime date { get; set; }   

        public bool IsAvailable { get; set; }


        [Precision(18,2)]
        public decimal price { get; set; }

        [ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }
        public Property Property{ get; set; }


    }
}
