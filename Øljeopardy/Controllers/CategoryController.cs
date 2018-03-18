using System;
using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Oljeopardy.DataAccess;
using Oljeopardy.Models;
using Oljeopardy.Models.JeopardyViewModels;
using DataTables.AspNet.AspNetCore;
using System.Collections.Generic;
using System.Linq;
using DataTables.AspNet.Core;

namespace Oljeopardy.Controllers
{
    public class CategoryController : Controller
    {
        private IMapper Mapper { get; set; }
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;

        public CategoryController(IMapper mapper, ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager, IUserRepository userRepository) 
        {
            Mapper = mapper;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _userRepository = userRepository;
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

        public IActionResult SharedCategories(IDataTablesRequest request)
        {
            // Nothing important here. Just creates some mock data.
            var userId = _userManager.GetUserId(HttpContext.User);
            var allOtherCategories = _categoryRepository.GetOtherCategories(userId);
            var data = new List<OtherCategoryViewModel>();
            foreach (var category in allOtherCategories)
            {
                var otherUser = _userRepository.GetUserById(category.UserId);
                if (otherUser == null)
                    continue;
                data.Add(new OtherCategoryViewModel()
                {
                    Category = category,
                    OwnerUserName = otherUser.UserName,
                    Saved = false
                });
            }

            // Global filtering.
            // Filter is being manually applied due to in-memmory (IEnumerable) data.
            // If you want something rather easier, check IEnumerableExtensions Sample.
            var filteredData = String.IsNullOrWhiteSpace(request.Search.Value)
                ? data
                : data.Where(x => x.OwnerUserName.Contains(request.Search.Value));

            var sortColumns = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumns != null)
            {
                var isAscending = sortColumns.Sort.Direction == SortDirection.Ascending;
                switch (sortColumns.Field)
                {
                    case "category.name":
                        filteredData = isAscending ? filteredData.OrderBy(x => x.Category.Name) : filteredData.OrderByDescending(x => x.Category.Name);
                        break;
                    case "ownerUserName":
                        filteredData = isAscending ? filteredData.OrderBy(x => x.OwnerUserName) : filteredData.OrderByDescending(x => x.OwnerUserName);
                        break;
                }
            }
            else
            {
                filteredData = filteredData.OrderBy(x => x.Category.Name);
            }

            // Paging filtered data.
            // Paging is rather manual due to in-memmory (IEnumerable) data.
            var dataPage = filteredData.Skip(request.Start).Take(request.Length);

            // Response creation. To create your response you need to reference your request, to avoid
            // request/response tampering and to ensure response will be correctly created.
            var response = DataTablesResponse.Create(request, data.Count(), filteredData.Count(), dataPage);

            // Easier way is to return a new 'DataTablesJsonResult', which will automatically convert your
            // response to a json-compatible content, so DataTables can read it when received.
            return new DataTablesJsonResult(response, true);
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
