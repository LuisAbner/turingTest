using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using turingPrueba.Models;
using turingPrueba.Permissions;
using turingPrueba.Service;

namespace turingPrueba.Controllers
{
    public class AdminController : Controller
    {
        private readonly IEditorialRepository _editorialRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger, IEditorialRepository editorialRepository, ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _editorialRepository = editorialRepository;
            _bookRepository = bookRepository;
        }
        public async Task<AdminViewModel> GetAdminViewModel()
        {
            var model = new AdminViewModel();
            var categories = await _categoryRepository.getAll();
            var booksBycategories = await _bookRepository.GetByCategory();
            var editorials = await _editorialRepository.getAll();
            if (categories is not null)
            {
                model.Categories = categories;
            }
            if (booksBycategories is not null)
            {
                model.BooksByCategories = booksBycategories;
            }
            if (editorials is not null)
            {
                model.Editorials = editorials;
            }
            return model;
        }
        [ValidateSession]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> Index()
        {
            var model = await GetAdminViewModel();
            return View(model);
        }
        [ValidateSession]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> AddCategory(Category category)
        {
            var model = await GetAdminViewModel();
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            TempData["response"] = await _categoryRepository.Add(category);
            TempData["type"] = 1;

            return RedirectToAction("Index", model);
        }
        [ValidateSession]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> AddEditorial(Editorial editorial)
        {
            var model = await GetAdminViewModel();
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            TempData["response"] = await _editorialRepository.Add(editorial);
            TempData["type"] = 1;

            return RedirectToAction("Index", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}