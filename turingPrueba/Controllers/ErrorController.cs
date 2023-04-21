using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace proyecto.Controllers;
public class ErrorController : Controller
{
    [Route("error/handle/{errorCode}")]
    public IActionResult Handle(int errorCode)
    {
        string nameError = "";
        switch(errorCode) 
        {
        case 403:
            nameError = "E403";
            break;
        case 404:
            nameError = "E404";
            break;
        case 500:
            nameError = "E500";
            break;
        case 501:
            nameError = "E501";
        break;
        default:
            nameError = "E500";
            break;
        }
        return RedirectToAction(nameError);
    }

    public IActionResult E404()
    {
        return View("404");
    }

   public IActionResult E403()
    {
        return View("403");
    }

    public IActionResult E500()
    {
        return View("500");
    }

     public IActionResult E501()
    {
        return View("501");
    }
}
