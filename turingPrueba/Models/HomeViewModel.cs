using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace turingPrueba.Models
{
    public class HomeViewModel:User
    {
        public List<SelectListItem> RoleList{get;set;}= new List<SelectListItem>();
        public UserLogin userLogin= new UserLogin();
    }
}