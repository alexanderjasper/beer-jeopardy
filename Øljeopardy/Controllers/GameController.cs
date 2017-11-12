using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Oljeopardy.DataAccess;
using Oljeopardy.Models;
using Oljeopardy.Models.JeopardyViewModels;

namespace Oljeopardy.Controllers
{
    public class GameController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;

        public GameController(ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager)
        {
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        public IActionResult Add()
        {
            ViewData["Title"] = "Start nyt spil";

            var userId = _userManager.GetUserId(HttpContext.User);
            var model = new AddGameViewModel()
            {
                CategoryList = _categoryRepository.GetCategoriesByUserId(userId)
            };

            return View(model);
        }

        public IActionResult Participate()
        {
            ViewData["Title"] = "Deltag i et spil";
            return View();
        }
    }
}