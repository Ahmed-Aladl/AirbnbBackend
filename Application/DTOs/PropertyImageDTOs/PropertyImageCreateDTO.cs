using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PropertyImageDTOs
{
    public class PropertyImageCreateDTO
    {
        public string GroupName { get; set; }
        public string ImageUrl { get; set; }
        public bool IsCover { get; set; }
    }

}
