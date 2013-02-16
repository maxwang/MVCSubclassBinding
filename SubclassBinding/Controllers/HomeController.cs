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
        public ActionResult Index(IEnumerable<Product> items)
        {
            return View("Result", items);
        }

        public ActionResult Welcome()
        {
            var model = new ProductView();
            var products = new List<Product>();
            products.Add(new HostedPbx());
            products.Add(new Voip());
            model.Products = products;
            return View(model);
        }

        [HttpPost]
        public ActionResult Welcome(ProductView items)
        {
            return View("WelcomeResult", items);
        }
    }
}
