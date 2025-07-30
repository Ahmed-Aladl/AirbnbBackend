using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums.PropertyViolations;

namespace Domain.Models
{

    public class PropertyViolations
    {

        public int Id { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PropertyViolationsStatus Status { get; set; } 
        public string? AdminNotes { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
        public Property Property { get; set; }  
    }
}


