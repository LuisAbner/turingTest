using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using turingPrueba.Models;
using turingPrueba.Service;

namespace turingPrueba.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;


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
        var model = await GetHomeViewModel();
        return View(model);
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
        TempData["type"] = 1;
        return View("Login", model);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
