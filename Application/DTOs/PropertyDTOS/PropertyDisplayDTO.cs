using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyImageDTOs;
using Domain.Enums.Property;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.DTOs.PropertyDTOS
{
    public class PropertyDisplayDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }

        [Precision(9, 6)]
        public decimal Latitude { get; set; }
        [Precision(9, 6)]
        public decimal Longitude { get; set; }

        [Precision(18, 2)]
        public decimal PricePerNight { get; set; }

        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Beds { get; set; }
        public int Bathrooms { get; set; }

        public float AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public bool isFavourite { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public PropertyAcceptStatus Status { get; set; } = PropertyAcceptStatus.Pending;
        public int PropertyTypeId { get; set; }
        public string HostId { get; set; }
        public List<PropertyImageDisplayDTO>? Images { get; set; }
        public string? HostPicture { get; set; }
    }
}
