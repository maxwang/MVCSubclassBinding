using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubclassBinding.Models
{
    public class HostedPbx : Product
    {
        public HostedPbx()
        {
            this.ProductType = this.GetType().Name;
        }
    }
}