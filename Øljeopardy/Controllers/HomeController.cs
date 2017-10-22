using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Øljeopardy.Data;
using Øljeopardy.Models;
using Øljeopardy.Models.JeopardyViewModels;

namespace Øljeopardy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Øljeopardy";

            return View();
        }

        public IActionResult Categories(CategoriesViewModel model)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            var categories = new List<Category>();
            using (_context)
            {
                var query = from c in _context.Categories
                    where c.UserId == userId
                    orderby c.Name
                    select c;
                foreach (var category in query)
                {
                    categories.Add(category);
                }
            }
            model.CategoryList = categories;

            var message = "";
            switch (model.PageAction)
            {
                case Enums.CategoriesPageAction.AddedCategory:
                    message = "Kategori tilføjet";
                    break;
                case Enums.CategoriesPageAction.EditedCategory:
                    message = "Kategori redigeret";
                    break;
            }
            ViewData["Message"] = message;
            ViewData["Title"] = "Kategorier";

            return View(model);
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

        public IActionResult Category(string message = null)
        {
            ViewData["Title"] = "Kategori";
            ViewData["Message"] = message;

            return View();
        }

        public IActionResult CreateCategory(CategoryViewModel model)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            var category = new Category()
            {
                Name = model.Name,
                UserId = userId,
                AnswerQuestion100 = new AnswerQuestion()
                {
                    Answer = model.AnswerQuestion100.Answer,
                    Question = model.AnswerQuestion100.Question
                },
                AnswerQuestion200 = new AnswerQuestion()
                {
                    Answer = model.AnswerQuestion200.Answer,
                    Question = model.AnswerQuestion200.Question
                },
                AnswerQuestion300 = new AnswerQuestion()
                {
                    Answer = model.AnswerQuestion300.Answer,
                    Question = model.AnswerQuestion300.Question
                },
                AnswerQuestion400 = new AnswerQuestion()
                {
                    Answer = model.AnswerQuestion400.Answer,
                    Question = model.AnswerQuestion400.Question
                },
                AnswerQuestion500 = new AnswerQuestion()
                {
                    Answer = model.AnswerQuestion500.Answer,
                    Question = model.AnswerQuestion500.Question
                }
            };
            _context.Add(category);
            _context.SaveChanges();

            ViewData["Title"] = "Kategorier";

            var categoriesModel = new CategoriesViewModel()
            {
                PageAction = Enums.CategoriesPageAction.AddedCategory
            };

            return RedirectToAction("Categories", categoriesModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
