using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MobileConnectDemo.Models;
using Newtonsoft.Json;

namespace MobileConnectDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult MobileConnectAuthorize(MobileConnectAuthorizeModel model)
        {
            return Content(JsonConvert.SerializeObject(model));
        }
    }
}