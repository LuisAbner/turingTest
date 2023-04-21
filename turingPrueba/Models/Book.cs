using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace turingPrueba.Models
{
    public class Book
    {
        public long IdBook{get;set;}
        public bool Status{get;set;}
        [Required(ErrorMessage = "Este es un campo obligatorio")]
        public string? Title{get;set;}
        public string? Subtitle{get;set;}
        [Required(ErrorMessage = "Este es un campo obligatorio")]
        [Range(1, 15000, 
        ErrorMessage = "Valor para {0} debe de estar entre {1} and {2}.")]
        public int Pages{get;set;}
        [Required(ErrorMessage = "Este es un campo obligatorio")]
        [StringLength(maximumLength: 13, MinimumLength=13, 
        ErrorMessage="* Debe de contener 13 caracteres.")]
        public string? Isbn{get;set;}

        [Required(ErrorMessage = "Este es un campo obligatorio")]
        public string? CategoryId{get;set;}
        [Required(ErrorMessage = "Este es un campo obligatorio")]
        public string? EditorialId{get;set;}        
        public string? UserId{get;set;}


        
    }
}