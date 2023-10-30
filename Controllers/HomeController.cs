using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LoginAndRegistration.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoginAndRegistration.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
    [HttpPost("user/create")]
    public IActionResult RegisterUser(User newUser)
    {
        if (ModelState.IsValid)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            return RedirectToAction("DashBoard");
        }
        else
        {
            return View("Index", newUser);
        }
    }
    [HttpPost("validar")]
    public IActionResult ValidaUser(LoginUser userLogin)
    {

        if (ModelState.IsValid)
        {
            User? UserExist = _context.users.FirstOrDefault(u => u.Email == userLogin.Email);
            if (UserExist == null)
            {
                ModelState.AddModelError("Email", "Correo o contraseña invalidos");
                return View("Index");
            }
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            var result = hasher.VerifyHashedPassword(userLogin, UserExist.Password, userLogin.Password);
            if (result == 0)
            {
                ModelState.AddModelError("Email", "Correo o contraseña invalidos");
                return View("Index");
            }
            HttpContext.Session.SetInt32("UserId", UserExist.UserId);
            return RedirectToAction("DashBoard");
        }
        else
        {
            return View("Index", userLogin);
        }
    }
    [SessionCheck]
    [HttpGet("success")]
    public IActionResult Dashboard()
    {
        return View("Dashboard");
    }

    [HttpPost("Logout")]
    public IActionResult LogOut()
    {
        HttpContext.Session.SetString("UserId", "");
        HttpContext.Session.Clear();
        return View("index");
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
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        //Encontrar la sesion 
        int? UserId = context.HttpContext.Session.GetInt32("UserId");
        if (UserId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}