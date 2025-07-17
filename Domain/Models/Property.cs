using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public decimal PricePerNight { get; set; }

        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Beds{ get; set; }
        public int Bathrooms { get; set; }

        public float AverageRating{ get; set; }
        public int ReviewCount { get; set; }
        
        public bool IsActive { get; set; }

        public int PropTypeId {  get; set; }
        public string HostId{ get; set; }

    }
}
