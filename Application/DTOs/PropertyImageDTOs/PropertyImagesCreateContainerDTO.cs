using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PropertyImageDTOs
{
    public class PropertyImagesCreateContainerDTO
    {
    
        public int PropertyId { get; set; }
        public string HostId { get; set; }
        public List<PropertyImageCreateDTO> Images { get; set; }
    }
}
