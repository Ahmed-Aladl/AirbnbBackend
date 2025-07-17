using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Amenity
    {
        public int Id { get; set; } 

        public string AmenityName { get; set; }
        public string IconURL  { get; set; }

    }
}
