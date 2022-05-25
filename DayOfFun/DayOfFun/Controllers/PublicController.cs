using System.Diagnostics;
using System.Net;
using DayOfFun.managers;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;
using Microsoft.AspNetCore.Mvc;

namespace DayOfFun.Controllers;

/// <summary>
/// Public controller for public access without registering
/// </summary>
public class PublicController : Controller
{
    private readonly ApplicationManager _applicationManager;

    public PublicController(ApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    /// <summary>
    /// GET Index for public usage
    /// </summary>
    /// <param name="email">email of public user -> cannot access without email</param>
    /// <returns>ERROR / VIEW </returns>
    public IActionResult Index(string email)
    {
        var emailVar = email == null ? HttpContext.Session.GetString("Email") : email;
        if (!_applicationManager.TemporaryLogin(emailVar, out var temoporaryUser))
            return RedirectToAction("Login", "Account");
        _applicationManager.GetQuizzesForUser(temoporaryUser, out var model);
        if (!HttpContext.Session.TryGetValue("Email", out _))
        {
            HttpContext.Session.SetString("Email", email);
        }

        return View(model);
    }

    /// <summary>
    /// GET public fill quiz. Need quizID as parameter and also email of the user
    /// </summary>
    /// <param name="id">quiz id</param>
    /// <returns>VIEW / ERROR</returns>
    public IActionResult Fill(int id)
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

    /// <summary>
    /// POST fill logic. If successfully filled, redirect to index. Otherwise write errors to user 
    /// </summary>
    /// <param name="model">filled questions for quiz</param>
    /// <returns>redirect to index if successes, otherwise return errors to user </returns>
    [HttpPost]
    public IActionResult Fill(QuizAnswerModel model)
    {
        if (ModelState.IsValid)
        {
            TempData["successMessage"] = "Quiz successfully filled. Thank you.";
            if (_applicationManager.TemporaryLogin(HttpContext.Session.GetString("Email"), out var u))
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

    /// <summary>
    /// GET Details about quiz - which users have filled it and so on.
    /// </summary>
    /// <param name="id">id of the quiz. Also needed is email (from session) to temporarily log user</param>
    /// <returns>Details about quizzes / errors to user</returns>
    public IActionResult Details(int id)
    {
        var emailVar = HttpContext.Session.GetString("Email");
        if (!_applicationManager.TemporaryLogin(emailVar, out var temoporaryUser))
            return RedirectToAction("Login", "Account");
        if (_applicationManager.GetQuizDetailsModel(temoporaryUser, id, out var qdm))
        {
            return View(qdm);
        }
        else
        {
            return RedirectToAction("Login", "Account");
        }
    }

    /// <summary>
    /// GET partial view - add question when filling
    /// </summary>
    /// <param name="quizId">id of the quiz</param>
    /// <returns> partial view for adding questions (to modal window)
    /// </returns>
    public IActionResult Update(int quizId)
    {
        return PartialView("_AddQuestionsModalPartialView", new Question());
    }

    /// <summary>
    /// POST - add question for quiz
    /// </summary>
    /// <param name="q">question to be added to the quiz</param>
    /// <returns>Response - OK if success, 500 if error. In the body is JSON with error/success message</returns>
    [HttpPost]
    public IActionResult Update(Question q)
    {
        var quizId = q.Id;
        q.Id = 0;
        var emailVar = HttpContext.Session.GetString("Email");
        if (_applicationManager.TemporaryLogin(emailVar, out var temoporaryUser))
        {
            if (_applicationManager.AddQuestion(temoporaryUser, q, quizId))
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

    /// <summary>
    /// Write error
    /// </summary>
    /// <returns>Error View</returns>
    public IActionResult Error()
    {
        return View(new ErrorViewModel() {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}