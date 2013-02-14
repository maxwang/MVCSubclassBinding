using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubclassBinding.Models
{
    public abstract class Product
    {
        public string Name { get; set; }
        public string ProductType { get; set; }
        public string DisplayOrder { get; set; }
    }
}