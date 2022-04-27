using DayOfFun.managers;
using DayOfFun.Model;
using DayOfFun.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

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
        User u;
        if (_applicationManager.TemporaryLogin(email, out u)) {
            QuizAnswerModel qam =_applicationManager.GetQuizFillModel(u, id);
            return View(qam);
        } 
        return RedirectToAction("Error");
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel());
    }

}