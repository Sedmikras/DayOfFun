using DayOfFun.Data.Services.Contract;
using DayOfFun.Model;
using DayOfFun.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DayOfFun.Controllers
{
    public class QuizController : Controller
    {
        private readonly IUserService _service;
        private readonly IQuizService _quizService;

        public QuizController(IUserService service, IQuizService quizService)
        {
            _service = service;
            _quizService = quizService;
        }

        public IActionResult Index()
        {
            var data = _quizService.getQuizzesForUser(_service.getUserFromSession(HttpContext.Session));
            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            Quiz quiz = new Quiz();
            quiz.ViewCollection.Add(new Question());
            return View(quiz);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Quiz quiz)
        {
            foreach (var question in quiz.ViewCollection)
            {
                question.Quizzes.Append(quiz);
            }

            User currentUser = _service.getUserFromSession(HttpContext.Session);
            quiz.Users.Append(currentUser);
            
            ModelState.Clear();
            TryValidateModel(quiz);
            
            if (!ModelState.IsValid)
            {
                return View(quiz);
            }
            
            _quizService.AddQuiz(quiz, HttpContext.Session);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            _quizService.Delete(HttpContext.Session,id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Fill(int id)
        {
            Quiz_Answer_Model model = _quizService.getQuestionsFor(HttpContext.Session, id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Fill(Quiz_Answer_Model model)
        {
            if (ModelState.IsValid)
            {
                _quizService.ValidateModel(model);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(model);
            }
        }

        /*[HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return null;
        }*/
    }
}