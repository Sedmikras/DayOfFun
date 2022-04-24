using DayOfFun.Data;
using DayOfFun.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DayOfFun.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
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
    public ActionResult Register(User userModel)
    {
        if (ModelState.IsValid)
        {
            if (userModel.Password != userModel.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords does not match");
                return View();
            }

            if (_context.Users.Where(user => user.Email == userModel.Email).FirstOrDefault() == null)
            {
                _context.Users.Add(userModel);
                _context.SaveChanges();
                ModelState.Clear();
                ViewBag.Message = userModel.Username + " with email [" + userModel.Email + "] succesfully registered";
            }
            else
            {
                ModelState.AddModelError("","User with email [" + userModel.Email + "] already exists!");
            }
        }

        return View();
    }

    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(User userModel)
    {
        if (userModel.Password != null && userModel.Email != null)
        {
            
            //check user
            User u = _context.Users.Where(user => user.Email == userModel.Email).FirstOrDefault();
            if (u.Password != userModel.Password)
            {
                ModelState.AddModelError("", "Username or Password is wrong");
            }
            else
            {
                //load from DB and check passwd and username 
                HttpContext.Session.SetString("UserId", u.Id.ToString());
                HttpContext.Session.SetString("Username", u.Username);
                HttpContext.Session.SetString("Email", u.Email);
                return RedirectToAction("LoggedIn");
            }
        }
        else
        {
            ModelState.AddModelError("", "Username or Password is wrong");
        }

        return View();
    }

    public ActionResult LoggedIn()
    {
        if (HttpContext.Session.GetString("UserId") != null)
        {
            return View();
        }
        else
        {
            return RedirectToAction("Login");
        }
    }

}