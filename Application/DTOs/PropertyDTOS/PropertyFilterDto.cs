using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums.Property;

namespace Application.DTOs.PropertyDTOS
{
    public class PropertyFilterDto
    {
        public string? Country { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public int? GuestsCount { get; set; }
        public DateTime? StartDate { get; set; }  // Nullable for specific or ranged
        public DateTime? EndDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public float maxDistanceKm { get; set; }
        public PropertyAcceptStatus Status { get; set; }

    }

}
