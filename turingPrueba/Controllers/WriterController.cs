using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using turingPrueba.Extensions;
using turingPrueba.Models;
using turingPrueba.Permissions;
using turingPrueba.Service;

namespace turingPrueba.Controllers
{
    public class WriterController : Controller
    {
        public const string SessionId = "_IdUser";
        private readonly ILogger<WriterController> _logger;
        private readonly IEditorialRepository _editorialRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;

        public WriterController(ILogger<WriterController> logger, IEditorialRepository editorialRepository, ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _editorialRepository = editorialRepository;
            _bookRepository = bookRepository;
        }
        public async Task<IndexWriterViewModel> GetModelIndex()
        {
            var idUser = HttpContext.Session.Get<int>(SessionId);
            var model = new IndexWriterViewModel();
            var editorialList = await _editorialRepository.getAll();
            var categoryList = await _categoryRepository.getAll();
            var books = await _bookRepository.GetByUSer(idUser);
            var categories = await _bookRepository.GetByCategory();
            if (editorialList is not null)
            {
                model.EditorialList = editorialList.Select(
                    x => new SelectListItem { Text = x.Name, Value = x.IdEditorial.ToString() }
                ).ToList();
            }
            if (categoryList is not null)
            {
                model.CategoryList = categoryList.Select(
                    x => new SelectListItem { Text = x.Name, Value = x.IdCategory.ToString() }
                ).ToList();
            }
            if (books is not null)
            {
                model.BooksByUser = books;
            }
            if (categories is not null)
            {
                model.BooksByCategories= categories;
            }
            return model;
        }

        [ValidateSession]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> Index()
        {
            var model = await GetModelIndex();
            return View(model);
        }
        [ValidateSession]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> AddBook(Book book)
        {
            var model = await GetModelIndex();
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            book.UserId = HttpContext.Session.Get<int>(SessionId).ToString();
            TempData["response"] = await _bookRepository.Save(book);
            TempData["type"] = 1;

            return RedirectToAction("Index", model);
        }

        [ValidateSession]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> Delete(long id)
        {
            var model = await GetModelIndex();
            TempData["response"] = await _bookRepository.DeleteById(id);
            TempData["type"] = 3;

            return RedirectToAction("Index", model);
        }

        [ValidateSession]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> UpdateBook(long id)
        {
            var model = await GetModelIndex();
            var book = await _bookRepository.GetById(id);
            if (book is not null)
            {
                model.IdBook = book.IdBook;
                model.Title = book.Title;
                model.Subtitle = book.Subtitle;
                model.Pages = book.Pages;
                model.Isbn = book.Isbn;
                model.CategoryId = book.CategoryId;
                model.EditorialId = book.EditorialId;
            }

            return View(model);
        }
        [ValidateSession]
        [Authorize(Roles = "writer")]
        [HttpPost]
        public async Task<IActionResult> UpdateBook(Book book)
        {
            var model = await GetModelIndex();
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }            
            TempData["response"] = await _bookRepository.Update(book);
            TempData["type"] = 2;
            return RedirectToAction("Index", model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}