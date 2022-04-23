using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DayOfFun.Data;
using DayOfFun.Data.Services.Contract;
using DayOfFun.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
            var data = _service.getQuizzesByUserId(1);
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

            quiz.Users.Append(_service.getUserByID(1));
            
            ModelState.Clear();
            TryValidateModel(quiz);
            
            if (!ModelState.IsValid)
            {
                return View(quiz);
            }
            //_quizService.AddQuiz(quiz);
            return RedirectToAction(nameof(Index));
        }
    }
}