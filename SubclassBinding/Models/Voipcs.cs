using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubclassBinding.Models
{
    public class Voip : Product
    {
        public string AreaCode { get; set; }
        public Voip()
        {
            this.ProductType = this.GetType().FullName;
        }
    }
}