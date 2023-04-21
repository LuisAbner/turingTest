using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace turingPrueba.Models
{
    public class IndexWriterViewModel:Book
    {
        public List<SelectListItem> CategoryList{get;set;}= new List<SelectListItem>();
        public List<SelectListItem> EditorialList{get;set;}= new List<SelectListItem>();
        public IEnumerable<Book> BooksByUser{get;set;} = new List<Book>();
        public IEnumerable<Category> BooksByCategories{get;set;} = new List<Category>();
    }
}