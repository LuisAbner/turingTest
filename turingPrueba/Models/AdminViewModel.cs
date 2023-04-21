using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace turingPrueba.Models
{
    public class AdminViewModel:Category
    {
        public IEnumerable<Category> BooksByCategories{get;set;} = new List<Category>();
        public IEnumerable<Editorial> Editorials{get;set;} = new List<Editorial>();
        public IEnumerable<Category> Categories{get;set;} = new List<Category>();
        public Editorial Editorial=new Editorial();
    }
}