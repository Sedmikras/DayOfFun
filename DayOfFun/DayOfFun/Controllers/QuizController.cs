using System.Diagnostics;
using System.Net;
using DayOfFun.managers;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;
using Microsoft.AspNetCore.Mvc;

namespace DayOfFun.Controllers
{
    /// <summary>
    /// Controller for Quizzes
    /// </summary>
    public class QuizController : Controller
    {
        private readonly ApplicationManager _applicationManager;

        private static QuizCreateViewModel _quizViewModel;

        public QuizController(ApplicationManager applicationManager)
        {
            _applicationManager = applicationManager;
        }

        /// <summary>
        /// GET Index page after login - quizzes for users
        /// </summary>
        /// <returns>View with all quizzes for user</returns>
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

        /// <summary>
        /// GET - Create quiz view
        /// </summary>
        /// <returns>Create View</returns>
        public IActionResult Create()
        {
            if (_quizViewModel != null) return View(_quizViewModel);
            _quizViewModel = new QuizCreateViewModel();
            _quizViewModel.Questions.Add(new Question());

            return View(_quizViewModel);
        }

        
        /// <summary>
        /// POST - create quiz
        /// </summary>
        /// <param name="quiz">quiz model with title and questions</param>
        /// <returns>redirect to index if success / error messages on view</returns>
        [HttpPost]
        public IActionResult Create(QuizCreateViewModel quiz)
        {
            if (ModelState.IsValid && _applicationManager.CreateQuiz(HttpContext.Session, quiz))
            {
                _quizViewModel = null;
                return RedirectToAction(nameof(Index));
            }

            TempData["errorMessage"] =
                "Cannot create quiz:" + quiz.Title + Environment.NewLine + "Questions were not filled!";
            _quizViewModel = quiz;
            return Create();
        }

        /// <summary>
        /// GET Delete action - for deleting quizzes from application
        /// </summary>
        /// <param name="id">quiz id</param>
        /// <returns>redirection to Index if success / back to view with errors</returns>
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

        /// <summary>
        /// GET - Fill action of the quiz (fill questions) 
        /// </summary>
        /// <param name="id">quiz ID</param>
        /// <returns>redirect to Fill view / error</returns>
        public IActionResult Fill(int id)
        {
            return !_applicationManager.GetQuizFillModel(HttpContext.Session, id, out var qam) ? Error() : View(qam);
        }

        /// <summary>
        /// POST - saves data from model to the DB (application)
        /// </summary>
        /// <param name="model">filled quiz (filled questions) </param>
        /// <returns>redirection to index if success / return to the page with errors</returns>
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
                TempData["errorMessage"] = "Cannot fill quiz";
                return View(model);
            }
        }

        /// <summary>
        /// GET - info about users that are participating in quiz
        /// </summary>
        /// <param name="quizId">id of the quiz</param>
        /// <returns>user info view / error</returns>
        public IActionResult Users(int quizId)
        {
            return !_applicationManager.GetQuizUsersView(HttpContext.Session, quizId, out var model) ? Error() : View(model);
        }

        /// <summary>
        /// Add question partial view
        /// </summary>
        /// <param name="quizId">id of the quiz</param>
        /// <returns>partial view used in modal window</returns>
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

        /// <summary>
        /// GET - Share controller - view to share quiz with users
        /// </summary>
        /// <param name="quizId">id of the quiz</param>
        /// <returns>partial view used in modal window</returns>
        public IActionResult Share(int quizId)
        {
            return PartialView("_ShareWithUserPartialView", new User());
        }

        /// <summary>
        /// POST - add user for the quiz. Used in modal window
        /// </summary>
        /// <param name="suv">model with user email and quiz id</param>
        /// <returns>Response - OK if success, 500 if error. In the body is JSON with error/success message</returns>
        [HttpPost]
        public IActionResult Share(ShareUserViewModel suv)
        {
            if (ModelState.IsValid)
            {
                _applicationManager.ShareQuiz(HttpContext.Session, suv);
                var successMessage = "Successfully added user with email:" + suv.Email;
                TempData["successMessage"] = successMessage;
                Response.StatusCode = (int) HttpStatusCode.OK;
                return Json(new {message = successMessage});
            }

            string errorMessage;
            errorMessage = suv.Email != null ? "Email is not in valid form !" : "Email is required";
            TempData["errorMessage"] = "ErrorMessage";
            Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return Json(new {message = errorMessage});
        }

        /// <summary>
        /// Should be HttpDelete but browser does not support it only from javascript and im kinda lazy to do it JS way
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <param name="quizId">id of the quiz</param>
        /// <returns>OK message if ok / error message if NOK</returns>
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

        /// <summary>
        /// GET Details about quiz - which users have filled it and so on.
        /// </summary>
        /// <param name="id">id of the quiz. Also needed is email (from session) to temporarily log user</param>
        /// <returns>Details about quizzes / errors to user</returns>
        public IActionResult Details(int id)
        {
            return !_applicationManager.GetQuizDetailsModel(HttpContext.Session, id, out var qdm) ? Error() : View(qdm);
        }

        /// <summary>
        /// GET - only real async method that suggest questions texts for user
        /// </summary>
        /// <returns>Status OK and in the body, question texts serialized to JSON</returns>
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

        /// <summary>
        /// Error view
        /// </summary>
        /// <returns>error view</returns>
        public IActionResult Error()
        {
            return View(new ErrorViewModel() {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}