﻿using System;
using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Oljeopardy.DataAccess;
using Oljeopardy.Models;
using Oljeopardy.Models.JeopardyViewModels;

namespace Oljeopardy.Controllers
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

            return PartialView("Category");
        }

        public IActionResult Edit(CategoriesViewModel categoriesViewModel)
        {
            ViewData["Title"] = "Rediger kategori";

            if (categoriesViewModel.ChosenCategoryGuid != null)
            {
                var category = _categoryRepository.GetCategoryById(categoriesViewModel.ChosenCategoryGuid.Value);
                var categoryViewModel = Mapper.Map<CategoryViewModel>(category);
                return PartialView("Category", categoryViewModel);
            }

            else
            {
                throw new Exception("No category chosen");
            }
        }

        public bool Delete(CategoriesViewModel categoriesViewModel)
        {
            ViewData["Title"] = "Rediger kategori";

            if (categoriesViewModel.ChosenCategoryGuid != null)
            {
                var category = _categoryRepository.GetCategoryById(categoriesViewModel.ChosenCategoryGuid.Value);
                return _categoryRepository.DeleteCategory(category, _userManager.GetUserId(HttpContext.User));
            }

            else
            {
                throw new Exception("No category chosen");
            }
        }

        public IActionResult SubmitCategory(CategoryViewModel categoryViewModel)
        {
            var category = Mapper.Map<Category>(categoryViewModel);
            var categoriesViewModel = new CategoriesViewModel();
            category.Name = category.Name.Trim();
            if (category.Name == "")
            {
                throw new Exception("Du skal navngive din kategori");
            }

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
