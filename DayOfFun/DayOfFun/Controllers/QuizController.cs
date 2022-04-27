using System.Security.Cryptography;
using System.Text;
using DayOfFun.managers;
using DayOfFun.Model;
using DayOfFun.Models.Domain;
using DayOfFun.Models.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
            qvm.questions.Add(new Question());
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
            _applicationManager.DeleteQuiz(HttpContext.Session,id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Fill(int id)
        {
            QuizAnswerModel qam =_applicationManager.GetQuizFillModel(HttpContext.Session, id);
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
        
        public IActionResult Share(int quizId)
        {
            String email = "Langmajerova.Barbora@gmail.com";
            email.ToLower();
            _applicationManager.Share(HttpContext.Session, email);
            /*byte[] hash;
            using (MD5 md5 = MD5.Create())
            {
                md5.Initialize();
                md5.ComputeHash(Encoding.UTF8.GetBytes(email));
                hash = md5.Hash;
            }

            var str = System.Text.Encoding.UTF8.GetString(hash);*/
            String adress = "\\public\\fill\\" + quizId + "?email=" + email;
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Edit(int id)
        {
            //Quiz quiz = _quizService.getQuizById(id);
            //quiz.ViewCollection.Add(new Question());
            return View();
        }
        
        public async Task<IActionResult> Details(int id)
        {
            QuizDetailsModel qdm =_applicationManager.GetQuizDetailsModel(HttpContext.Session, id);
            return View(qdm);
        }
    }
}