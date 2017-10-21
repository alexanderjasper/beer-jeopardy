using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Øljeopardy.Models;

namespace Øljeopardy.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Øljeopardy";

            return View();
        }

        public IActionResult Categories()
        {
            ViewData["Title"] = "Kategorier";

            return View();
        }

        public IActionResult Rules()
        {
            ViewData["Title"] = "Regler";

            return View();
        }

        public IActionResult Game()
        {
            ViewData["Title"] = "Spil";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
