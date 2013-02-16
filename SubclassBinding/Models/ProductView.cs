using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubclassBinding.Models
{
    public class ProductView
    {
        public IEnumerable<Product> Products { get; set; }
        public MyInformation Customer { get; set; }

        public ProductView()
        {
            Products= new List<Product>();
            this.Customer = new MyInformation();
        }
    }
}