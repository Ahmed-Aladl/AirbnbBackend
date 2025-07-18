using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class GetAllQueryDto
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 10;
    }
}
