using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace turingPrueba.Models
{
    public class User
    {
        public long IdUser{get; set;}

        [Required(ErrorMessage = "Este es un campo obligatorio")]
        [EmailAddress(ErrorMessage ="Utiliza el formato de correo válido")]
        public string? Email{get; set;}

        [Required(ErrorMessage = "Este es un campo obligatorio")]
        public string? Name{get;set;}

        [Required(ErrorMessage = "Este es un campo obligatorio")]
        public string? Password{get;set;}
        
        [Required(ErrorMessage = "Este es un campo obligatorio")]
        public string? Role{get;set;}
    }
}