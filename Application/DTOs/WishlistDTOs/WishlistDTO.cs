using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.WishlistDTOs
{
    public class WishlistDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public List<int> PropertyIds { get; set; }
    }

}
