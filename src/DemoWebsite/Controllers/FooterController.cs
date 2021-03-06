﻿using Lightcore.Kernel;
using Microsoft.AspNet.Mvc;

namespace DemoWebsite.Controllers
{
    public class FooterController : Controller
    {
        public ActionResult Index()
        {
            var context = HttpContext.LightcoreContext();

            return View("/Views/Footer/Index.cshtml", context.Item);
        }
    }
}