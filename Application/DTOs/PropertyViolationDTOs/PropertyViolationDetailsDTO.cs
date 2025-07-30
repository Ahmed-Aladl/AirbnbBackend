using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PropertyViolationDTOs
{
    public class PropertyViolationDetailsDTO

    {
        public int Id { get; set; }

        public string Reason { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }  

        public string? AdminNotes { get; set; }

        public string UserId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserEmail { get; set; }

        public int PropertyId { get; set; }

        public string PropertyTitle { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

    }
}
