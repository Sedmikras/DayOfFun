using System.Diagnostics;
using DayOfFun.managers;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;
using Microsoft.AspNetCore.Mvc;

namespace DayOfFun.Controllers;

public class PublicController : Controller
{
    private readonly ApplicationManager _applicationManager;

    public PublicController(ApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Fill(int id)
    {
        string email = Request.Query["email"];
        User? u;
        QuizAnswerModel qam;
        if (_applicationManager.TemporaryLogin(email, out u)) {
            HttpContext.Session.SetString("Email", email);
            _applicationManager.GetQuizFillModel(u, id, out qam);
            return View(qam);
        } 
        return RedirectToAction("Error");
    }

    [HttpPost]
    public async Task<IActionResult> Fill(QuizAnswerModel model)
    {
        if (ModelState.IsValid)
        {
            TempData["successMessage"] = "Quiz successfully filled. Thank you.";
            User? u;
            if (_applicationManager.TemporaryLogin(HttpContext.Session.GetString("Email"), out u))
            {
                _applicationManager.UpdateQuiz(u, model);    
            }
            return RedirectToAction(nameof(Index));
        }
        else
        {
            return View(model);
        }
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel() {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }

}