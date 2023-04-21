using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using turingPrueba.Models;
using turingPrueba.Service;
using turingPrueba.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace turingPrueba.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public const string SessionId = "_IdUser";
    public const string SessionEmail = "_Email";
    public const string SessionName = "_Name";
    public const string SessionStatus = "_Status";
    public const string SessionTypeRole = "_TypeRole";

    public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public IActionResult Index()
    {
        return View();
    }
    public async Task<HomeViewModel> GetHomeViewModel()
    {
        var model = new HomeViewModel();
        var roleList = await _roleRepository.getAll();
        if (roleList != null)
        {
            model.RoleList = roleList.Select(
                x => new SelectListItem { Text = x.Authority, Value = x.IdRole.ToString() }
            ).ToList();
        }
        return model;
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        
        if (HttpContext.Session.Get<long>(SessionId) != 0)
        {
            switch (HttpContext.Session.Get<string>(SessionTypeRole))
            {
                case "writer":
                    return RedirectToAction("Writer", "Index");
                case "admin":
                    return RedirectToAction("Admin", "Index");                
                default:
                    return RedirectToAction("Login");
            }
        }
        var model = await GetHomeViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLogin userLogin)
    {        

        var id = await _userRepository.LoginUser(userLogin);
        
        Console.WriteLine(id);

        DataSession dataSession;
        if (id != 0)
        {
            var dataSessionGet = await _userRepository.GetDataSession(id);
            if (dataSessionGet is not null)
            {
                if (!dataSessionGet.Status)
                {
                    TempData["response"] = 1;
                    return RedirectToAction("Login");
                }
                dataSession = dataSessionGet;
                HttpContext.Session.Set<int>(SessionId, Convert.ToInt32(id));
                HttpContext.Session.Set<string>(SessionEmail, dataSession.Email);
                HttpContext.Session.Set<string>(SessionName, dataSession.Name);
                HttpContext.Session.Set<string>(SessionTypeRole, dataSession.TypeRole);

                #region AUTENTICACTION
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, dataSession.Name),
                    new Claim("Username", dataSession.Email),
                    new Claim(ClaimTypes.Role, dataSession.TypeRole)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                #endregion

                if (dataSession.Status)
                {
                    switch (dataSession.TypeRole)
                    {
                        case "writer":
                            TempData["type"] = 4;
                            TempData["response"] = id;
                            return RedirectToAction("Index", "Writer");
                        case "admin":
                            TempData["type"] = 4;
                            TempData["response"] = id;
                            return RedirectToAction("Index", "Admin");
                        default:
                            return RedirectToAction("Login");
                    }
                }
                else
                {
                    TempData["response"] = 1;
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        else
        {
            TempData["response"] = 0;
            return RedirectToAction("Login");
        }
    }
    [HttpPost]
    public async Task<IActionResult> SaveUser(User user)
    {
        var model = await GetHomeViewModel();
        if (!ModelState.IsValid)
        {
            return View("Login", model);
        }
        TempData["response"] = await _userRepository.Register(user);
        TempData["type"] = 3;
        return View("Login", model);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SingOff()
    {
        HttpContext.Session.Clear();
        #region AUTENTICACTION
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        #endregion
        return RedirectToAction("Login");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}