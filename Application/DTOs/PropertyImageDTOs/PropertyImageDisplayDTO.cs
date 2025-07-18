using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PropertyImageDTOs
{
    public class PropertyImageDisplayDTO
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int PropertyId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsCover { get; set; }
        public bool IsDeleted { get; set; }
    }
}
