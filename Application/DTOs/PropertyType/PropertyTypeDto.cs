using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
namespace Application.DTOs.PropertyType
{
    public class PropertyTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconURL { get; set; }

        //public ICollection<Property> Properties { get; set; }



    }
}
