﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums.Property;
using Domain.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models
{
    public class Property
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

        public decimal WeekendPrice { get; set; }

        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Beds { get; set; }
        public int Bathrooms { get; set; }
        public float AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(PropertyType))]
        public int PropertyTypeId { get; set; }

        [ForeignKey(nameof(Host))]
        public string HostId { get; set; }
        public PropertyAcceptStatus Status { get; set; } = PropertyAcceptStatus.Pending;

        // Navigation properties
        public PropertyType PropertyType { get; set; }
        public User Host { get; set; }
        public ICollection<Amenity> Amenities { get; set; }
        public ICollection<PropertyAmenity> PropertyAmenities { get; set; }
        public ICollection<CalendarAvailability> CalendarAvailabilities { get; set; } =
            new List<CalendarAvailability>();

        public ICollection<WishlistProperty> WishlistProperties { get; set; }
        public ICollection<PropertyImage> Images { get; set; }
        public ICollection<Booking> Bookings { get; set; }

        public ICollection<ChatSession> ChatSessions{ get; set; }
    }
}
