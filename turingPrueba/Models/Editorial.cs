using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace turingPrueba.Models
{
    public class Editorial
    {
        public long IdEditorial{get;set;}
        [Required(ErrorMessage = "Este es un campo obligatorio")]
        public string? Name{get;set;}
    }
}