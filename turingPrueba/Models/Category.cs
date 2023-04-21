using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace turingPrueba.Models
{
    public class Category
    {
        public long IdCategory{get;set;}
        public string? Name{get;set;}
        [Required(ErrorMessage = "Este es un campo obligatorio")]
        public List<Book> books {get;set;}=new List<Book>();
    }
}