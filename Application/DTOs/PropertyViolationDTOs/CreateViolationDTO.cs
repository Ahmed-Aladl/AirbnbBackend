using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PropertyViolationDTOs
{
    public class CreateViolationDTO
    {
        public string Reason { get; set; } 
        public string UserId { get; set; } 
        public int PropertyId { get; set; }

    }
}
