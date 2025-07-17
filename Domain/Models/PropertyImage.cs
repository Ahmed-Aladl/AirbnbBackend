using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PropertyImage
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Property))]
        public int PropertyId{ get; set; }
        public string ImageUrl{ get; set; }
        public bool IsCover{ get; set; }

        public Property Property { get; set; }

        public bool IsDeleted { get; set; }
    }
}
