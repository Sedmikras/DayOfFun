using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DayOfFun.Data;
using DayOfFun.Data.Services.Contract;
using Microsoft.AspNetCore.Mvc;

namespace DayOfFun.Controllers
{
    public class QuizController : Controller
    {
        private readonly IUserService _service;

        public QuizController(IUserService service)
        {
            _service = service;
        }
        
        public IActionResult Index()
        {
            var data = _service.getQuizzesByUserId(1);
            return View(data);
        }
    }
}