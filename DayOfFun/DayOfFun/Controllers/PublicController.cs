using System.Diagnostics;
using System.Net;
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
    public IActionResult Index(string email)
    {
        string emailVar = email == null ? HttpContext.Session.GetString("Email") : email;
        if (_applicationManager.TemporaryLogin(emailVar, out var temoporaryUser))
        {
            _applicationManager.GetQuizzesForUser(temoporaryUser, out var model);
            if (!HttpContext.Session.TryGetValue("Email", out _))
            {
                HttpContext.Session.SetString("Email", email);
            }

            return View(model);
        }

        return RedirectToAction("Login", "Account");
    }

    public async Task<IActionResult> Fill(int id)
    {
        string email = Request.Query["email"];
        User? u;
        QuizAnswerModel qam;
        if (_applicationManager.TemporaryLogin(email, out u))
        {
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

    public IActionResult Details(int id)
    {
        string emailVar = HttpContext.Session.GetString("Email");
        if (_applicationManager.TemporaryLogin(emailVar, out var temoporaryUser))
        {
            if (_applicationManager.GetQuizDetailsModel(temoporaryUser, id, out var qdm))
            {
                return View(qdm);
            }
            else
            {
                return RedirectToAction("Login","Account");
            }
        }

        return RedirectToAction("Login","Account");
    }
    
    public IActionResult Update(int quizId)
    {
        return PartialView("_AddQuestionsModalPartialView", new Question());
    }

    [HttpPost]
    public IActionResult Update(Question q)
    {
        var quizId = q.Id;
        q.Id = 0;
        var emailVar = HttpContext.Session.GetString("Email");
        if (_applicationManager.TemporaryLogin(emailVar, out var temoporaryUser) )
        {
            if(_applicationManager.AddQuestion(temoporaryUser, q, quizId))
            {
                var successMessage = "Successfully added question:" + q.Text;
                Response.StatusCode = (int) HttpStatusCode.OK;
                return Json(new {message = successMessage});
               
            }    
        }
        var errorMessage = "cannot add question for quiz " + quizId;
        Response.StatusCode = (int) HttpStatusCode.InternalServerError;
        return Json(new {message = errorMessage});
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel() {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}