using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using turingPrueba.Extensions;

namespace turingPrueba.Permissions
{
    public class ValidateSessionAttribute : ActionFilterAttribute
    {
        public const string SessionId = "_IdUser";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var idUserSession = filterContext.HttpContext.Session.Get<int>(SessionId);
            if (idUserSession == 0)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}