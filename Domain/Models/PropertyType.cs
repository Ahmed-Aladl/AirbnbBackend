﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PropertyType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconURL { get; set; }


        public ICollection<Property> Properties { get; set; }
    }
}
