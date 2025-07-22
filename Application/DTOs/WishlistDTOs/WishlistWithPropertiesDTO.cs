using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyDTOS;

namespace Application.DTOs.WishlistDTOs
{
    public class WishlistWithPropertiesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public List<PropertyDisplayDTO> Properties { get; set; } = new();
    }

}
