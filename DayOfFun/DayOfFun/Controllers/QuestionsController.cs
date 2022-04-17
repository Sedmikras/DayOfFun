using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DayOfFun.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DayOfFun.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            var data = await _context.quizes.ToListAsync();
            var questions = data[0].Questions;
            return View();
        }
    }
}