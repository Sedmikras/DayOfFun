using System.Net;
using DayOfFun.managers;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace DayOfFun.Controllers
{
    public class QuizController : Controller
    {
        private readonly ApplicationManager _applicationManager;

        public QuizController(ApplicationManager applicationManager)
        {
            _applicationManager = applicationManager;
        }

        public IActionResult Index()
        {
            if (_applicationManager.GetQuizzesForUser(HttpContext.Session, out var model))
            {
                return View(model);
            }
            else
            {
                TempData["errorMessage"] = "Couldn't read quizzes for user";
                return RedirectToAction("Error");
            }
        }

        public IActionResult Create()
        {
            var qvm = new QuizCreateViewModel();
            qvm.Questions.Add(new Question());
            return View(qvm);
        }

        [HttpPost]
        public IActionResult Create(QuizCreateViewModel quiz)
        {
            if (_applicationManager.CreateQuiz(HttpContext.Session, quiz)) return RedirectToAction(nameof(Index));
            TempData["errorMessage"] = "Couldn't create quiz with tittle " + quiz.Title;
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (_applicationManager.DeleteQuiz(HttpContext.Session, id))
            {
                return RedirectToAction(nameof(Index));    
            }
            else
            {
                TempData["errorMessage"] = "Couldn't delete quiz with id " + id;
                return RedirectToAction("Error");
            }
        }

        public IActionResult Fill(int id)
        {
            //TODO
            var qam = _applicationManager.GetQuizFillModel(HttpContext.Session, id);
            return View(qam);
        }

        [HttpPost]
        public IActionResult Fill(QuizAnswerModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["successMessage"] = "Quiz successfully filled. Thank you.";
                _applicationManager.UpdateQuiz(HttpContext.Session, model);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult Users(int quizId)
        {
            //TODO
            return View(_applicationManager.getQuizUsersView(HttpContext.Session, quizId));
        }

        public IActionResult Share(int quizId)
        {
            return PartialView("_ShareWithUserPartialView", new User());
        }

        [HttpPost]
        public IActionResult Share(ShareUserViewModel suv)
        {
            //TODO
            if (ModelState.IsValid)
            {
                _applicationManager.ShareQuiz(HttpContext.Session, suv);
                var successMessage = "Successfully added user with email:" + suv.Email;
                TempData["successMessage"] = successMessage;
                Response.StatusCode = (int) HttpStatusCode.OK;
                return Json(new { message = successMessage });
            }

            //  When I want to return error:
            Response.StatusCode = (int) HttpStatusCode.OK;
            return Json("Message sent");
        }

        /// <summary>
        /// Should be HttpDelete but browser does not support it only from javascript and im kinda lazy to do it JS way
        /// </summary>
        /// <param name="email"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        public IActionResult Exclude(string  email, int quizId)
        {
            if (!_applicationManager.RemoveUser(email, quizId))
            {
                TempData["errorMessage"] = "Failed to remove user";
            }
            else
            {
                TempData["successMessage"] = "User successfully removed from quiz";    
            }
            return RedirectToAction("Users", new {quizId});
        }

        public IActionResult Details(int id)
        {
            //TODO
            var qdm = _applicationManager.GetQuizDetailsModel(HttpContext.Session, id);
            return View(qdm);
        }

        [Produces("application/json")]
        public async Task<IActionResult> Suggest()
        {
            var term = HttpContext.Request.Query["term"].ToString();
            var questionTexts = await _applicationManager.SuggestQuestionsAsync(HttpContext.Session, term);
            // Convert the suggested query results to a list that can be displayed in the client.

            // Return the list of suggestions.
            return Ok(questionTexts);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel());
        }
    }
}