using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class WishlistProperty
    {
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
