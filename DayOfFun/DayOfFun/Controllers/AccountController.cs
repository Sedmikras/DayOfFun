using DayOfFun.Data;
using DayOfFun.Data.Services.Contracts;
using DayOfFun.Models.View;
using Microsoft.AspNetCore.Mvc;

namespace DayOfFun.Controllers;

/// <summary>
/// Login / Logout controller 
/// </summary>
public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;

    public AccountController(ApplicationDbContext context, IUserService service)
    {
        _context = context;
        _userService = service;
    }

    /// <summary>
    /// Writes list of users -> its nice security leak... well as this whole program
    /// </summary>
    /// <returns>
    /// View with all users
    /// </returns>
    public ActionResult Index()
    {
        return View(_context.Users.ToList());
    }

    /// <summary>
    /// Register screen
    /// </summary>
    /// <returns>Register view</returns>
    public ActionResult Register()
    {
        return View();
    }

    /// <summary>
    /// Register post method in controller - try to register user
    /// </summary>
    /// <param name="userModel">info about new user - email, password, username</param>
    /// <returns>redirection back to view with error message or to login screen if success</returns>
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

    /// <summary>
    /// Login controller - get login screen
    /// </summary>
    /// <returns>login screen</returns>
    public ActionResult Login()
    {
        return View();
    }

    /// <summary>
    /// Login logic. POST method.
    /// </summary>
    /// <param name="userModel">info about user - email, password</param>
    /// <returns>redirect to index if success, error otherwise</returns>
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

    /// <summary>
    /// Logout logic
    /// </summary>
    /// <returns>redirect to login screen</returns>
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
}