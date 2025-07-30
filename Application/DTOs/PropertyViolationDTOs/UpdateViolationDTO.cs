using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PropertyViolationDTOs
{
    public class UpdateViolationDTO
    {
        public int Id { get; set; }
        public string? AdminNotes { get; set; }
        public string Status { get; set; }

    }
}
