using System.Diagnostics;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Oljeopardy.Data;
using Oljeopardy.DataAccess;
using Oljeopardy.Models;
using Oljeopardy.Models.JeopardyViewModels;

namespace Oljeopardy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMapper Mapper { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager, ICategoryRepository categoryRepository)
        {
            _context = context;
            Mapper = mapper;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Øljeopardy";

            return View();
        }

        public IActionResult Categories(Enums.CategoriesPageAction pageAction)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var model = new CategoriesViewModel()
            {
                PageAction = pageAction,
                CategoryList = _categoryRepository.GetCategoriesByUserId(userId)
            };

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
            var category = Mapper.Map<CategoryViewModel,Category>(model);
            _categoryRepository.AddCategory(category, userId);

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
