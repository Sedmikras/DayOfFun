using System.Net;
using System.Text.Json;
using DayOfFun.managers;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;
using Microsoft.AspNetCore.Mvc;

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
            var data = _applicationManager.GetQuizzesForUser(HttpContext.Session);
            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            QuizCreateViewModel qvm = new QuizCreateViewModel();
            qvm.Questions.Add(new Question());
            return View(qvm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuizCreateViewModel quiz)
        {
            _applicationManager.CreateQuiz(HttpContext.Session, quiz);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            _applicationManager.DeleteQuiz(HttpContext.Session, id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Fill(int id)
        {
            QuizAnswerModel qam = _applicationManager.GetQuizFillModel(HttpContext.Session, id);
            return View(qam);
        }

        [HttpPost]
        public async Task<IActionResult> Fill(QuizAnswerModel model)
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
            return View(_applicationManager.getQuizUsersView(HttpContext.Session, quizId));
        }

        public IActionResult Share(int quizId)
        {
            return PartialView("_ShareWithUserPartialView", new User());
        }

        [HttpPost]
        public async Task<IActionResult> Share(ShareUserViewModel suv)
        {
            if (ModelState.IsValid)
            {
                _applicationManager.ShareQuiz(HttpContext.Session, suv);
                string successMessage = "Succesfully added" + suv.Email;
                //return await _applicationManager.ValidateEmail(HttpContext, suv);

                //_applicationManager.ValidateEmail();
                //ViewBag["successMessage"] = successMessage;
                //  When I want to return success:
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json(successMessage, new JsonSerializerOptions(JsonSerializerOptions.Default));
            }

            /*if (!_applicationManager.ValidateEmail(HttpContext.Session, suv))
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json("Message sent");
            }*/

            //  When I want to return error:
            Response.StatusCode = (int) HttpStatusCode.OK;
            return Json("Message sent");
        }

        /// <summary>
        /// Should be HttpDelete but browser does not support it only from javascript and im kinda lazy to do it JS way
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="QuizId"></param>
        /// <returns></returns>
        public IActionResult Unshare(int UserId, int QuizId)
        {
            
            return RedirectToAction("Users", new {quizId = QuizId});
        }

        public async Task<IActionResult> Edit(int id)
        {
            //Quiz quiz = _quizService.getQuizById(id);
            //quiz.ViewCollection.Add(new Question());
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            QuizDetailsModel qdm = _applicationManager.GetQuizDetailsModel(HttpContext.Session, id);
            return View(qdm);
        }

        [Produces("application/json")]
        public async Task<IActionResult> Suggest()
        {
            var term = HttpContext.Request.Query["term"].ToString();
            List<String> questionTexts = await _applicationManager.SuggestQuestionsAsync(HttpContext.Session, term);
            // Convert the suggested query results to a list that can be displayed in the client.

            // Return the list of suggestions.
            return Ok(questionTexts);
        }
    }
}