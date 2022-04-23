using DayOfFun.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DayOfFun.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login(string returnUrl)
        {
            return View(new User());
        }


        [HttpPost]
        public async Task<IActionResult> Login(User userModel)
        {
            if (!ModelState.IsValid)
                return View(userModel);
            var user = await _userManager.FindByNameAsync(userModel.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, userModel.PasswordHash, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Username/password not found");
            return View(userModel);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User userModel)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser() {UserName = userModel.Name};
                //var result = await _userManager.CreateAsync(user, userModel.Password);

                //if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(userModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}