using Application.DTOs.Calendar;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    public class CalendarMappingProfile : Profile
    {
        public CalendarMappingProfile()
        {
            CreateMap<CalendarAvailability, CalendarAvailabilityDto>();
            CreateMap<CalendarUpdateDto, CalendarAvailability>();
        }
    }
}
