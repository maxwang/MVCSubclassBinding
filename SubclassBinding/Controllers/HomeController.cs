using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SubclassBinding.Models;

namespace SubclassBinding.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var products = new List<Product>();
            products.Add(new HostedPbx());
            products.Add(new Voip());
            return View(products);
        }

        [HttpPost]
        public ActionResult Index([ModelBinder(typeof(ProductBinder))] IEnumerable<Product> items)
        {
            return View("Result", items);
        }

    }
}
