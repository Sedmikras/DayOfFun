using System.Diagnostics;
using System.Net;
using DayOfFun.managers;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;
using Microsoft.AspNetCore.Mvc;

namespace DayOfFun.Controllers
{
    public class QuizController : Controller
    {
        private readonly ApplicationManager _applicationManager;

        private static QuizCreateViewModel quizViewModel;

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
            if (quizViewModel == null)
            {
                quizViewModel = new QuizCreateViewModel();
                quizViewModel.Questions.Add(new Question());
            }

            return View(quizViewModel);
        }

        [HttpPost]
        public IActionResult Create(QuizCreateViewModel quiz)
        {
            if (ModelState.IsValid && _applicationManager.CreateQuiz(HttpContext.Session, quiz))
            {
                quizViewModel = null;
                return RedirectToAction(nameof(Index));
            }

            TempData["errorMessage"] =
                "Cannot create quiz:" + quiz.Title + Environment.NewLine + "Questions were not filled!";
            quizViewModel = quiz;
            return Create();
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
            _applicationManager.GetQuizFillModel(HttpContext.Session, id, out var qam);
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
            _applicationManager.GetQuizUsersView(HttpContext.Session, quizId, out var model);
            return View(model);
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
            if (!_applicationManager.AddQuestion(HttpContext.Session, q, quizId))
            {
                var errorMessage = "cannot add question for quiz " + quizId;
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                return Json(new {message = errorMessage});
            }
            else
            {
                var successMessage = "Successfully added question:" + q.Text;
                Response.StatusCode = (int) HttpStatusCode.OK;
                return Json(new {message = successMessage});
            }
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
                return Json(new {message = successMessage});
            }

            string errorMessage;
            if (suv.Email != null)
            {
                errorMessage = "Email is not in valid form !";
            }
            else
            {
                errorMessage = "Email is required";
            }
            TempData["errorMessage"] = "ErrorMessage";
            Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return Json(new {message = errorMessage});
        }

        /// <summary>
        /// Should be HttpDelete but browser does not support it only from javascript and im kinda lazy to do it JS way
        /// </summary>
        /// <param name="email"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        public IActionResult Exclude(string email, int quizId)
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
            _applicationManager.GetQuizDetailsModel(HttpContext.Session, id, out var qdm);
            return View(qdm);
        }

        [Produces("application/json")]
        public async Task<IActionResult> Suggest()
        {
            return await Task.Run(() =>
            {
                var term = HttpContext.Request.Query["term"].ToString();
                var questionTexts = _applicationManager.SuggestQuestionsAsync(HttpContext.Session, term).Result;
                return Ok(questionTexts);
            });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel() {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}