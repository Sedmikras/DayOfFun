using DayOfFun.Data;
using DayOfFun.Data.Services.Contract;
using DayOfFun.Models.View;
using Microsoft.AspNetCore.Mvc;

namespace DayOfFun.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;

    public AccountController(ApplicationDbContext context, IUserService service)
    {
        _context = context;
        _userService = service;
    }

    public ActionResult Index()
    {
        return View(_context.Users.ToList());
    }

    public ActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Register(UserViewModel userModel)
    {
        if (ModelState.IsValid)
        {
            if (userModel.Password != userModel.ConfirmPassword)
            {
                TempData["errorMessage"] = "failed to register";
                ModelState.AddModelError("", "Passwords does not match");
                return View();
            }

            if (!_userService.RegisterUser(userModel))
            {
                TempData["errorMessage"] = "user with email already exists";
                ModelState.AddModelError("", "Cannot register user with email " + userModel.Email);
                return View();
            }
        }

        TempData["successMessage"] = "successfully registered";
        return RedirectToAction(nameof(Login));
    }

    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(UserViewModel userModel)
    {
        if (userModel.Password != null && userModel.Email != null)
        {
            //check user
            var u = _context.Users.FirstOrDefault(user => user.Email == userModel.Email);
            if (u == null || u.Password != userModel.Password)
            {
                ModelState.AddModelError("", "Username or Password is wrong");
                TempData["errorMessage"] = "Username or Password is wrong";
            }
            else
            {
                //load from DB and check passwd and username 
                HttpContext.Session.SetString("UserId", u.Id.ToString());
                HttpContext.Session.SetString("Username", u.Username);
                HttpContext.Session.SetString("Email", u.Email);
                TempData["successMessage"] = "Welcome back " + u.Username;
                return RedirectToAction("Index", "Quiz");
            }
        }
        else
        {
            ModelState.AddModelError("", "Username or Password is wrong");
        }

        return View();
    }
}