using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Øljeopardy.Data;
using Øljeopardy.DataAccess;
using Øljeopardy.Models;
using Øljeopardy.Models.JeopardyViewModels;

namespace Øljeopardy.Controllers
{
    public class CategoryController : Controller
    {
        private IMapper Mapper { get; set; }
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public CategoryController(IMapper mapper, ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager) 
        {
            Mapper = mapper;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        public IActionResult Create(string message = null)
        {
            ViewData["Title"] = "Opret kategori";
            ViewData["Message"] = message;

            return View("Category");
        }

        [HttpPost]
        public IActionResult Edit(CategoriesViewModel categoriesViewModel, string message = null)
        {
            ViewData["Title"] = "Rediger kategori";
            ViewData["Message"] = message;

            var category = _categoryRepository.GetCategoryById(categoriesViewModel.ChosenCategoryGuid);

            var categoryViewModel = Mapper.Map<CategoryViewModel>(category);

            return View("Category", categoryViewModel);
        }

        public IActionResult SubmitCategory(CategoryViewModel categoryViewModel)
        {
            var category = Mapper.Map<Category>(categoryViewModel);
            var categoriesViewModel = new CategoriesViewModel();

            if (categoryViewModel.Id != Guid.Empty)
            {
                _categoryRepository.UpdateCategory(category, _userManager.GetUserId(HttpContext.User));
                categoriesViewModel.PageAction = Enums.CategoriesPageAction.EditedCategory;
            }
            else
            {
                _categoryRepository.AddCategory(category, _userManager.GetUserId(HttpContext.User));
                categoriesViewModel.PageAction = Enums.CategoriesPageAction.AddedCategory;
            }

            ViewData["Title"] = "Kategorier";

            return RedirectToAction("Categories", "Home",  categoriesViewModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
