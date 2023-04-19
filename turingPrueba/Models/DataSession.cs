using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace turingPrueba.Models
{
    public class DataSession
    {
        public long IdUser{get;set;}
        public string? Name{get;set;}="";
        public string? Email{get;set;}="";
        public bool Status{get;set;}
        public string? TypeRole{get;set;}="";
        public long IdRole{get;set;}
    }
}