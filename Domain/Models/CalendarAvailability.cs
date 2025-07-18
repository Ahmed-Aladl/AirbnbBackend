// In Domain/Models/CalendarAvailability.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models
{
    public class CalendarAvailability
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Precision(18, 2)]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }
        public Property Property { get; set; }

        // Helper method to check if date is in the past
        public bool IsPastDate => Date < DateTime.Today;
    }
}