using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PropertyAmenity
    {
        [ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }
        public int AmenityId { get; set; }

        public Property Property { get; set; }
    }
}
