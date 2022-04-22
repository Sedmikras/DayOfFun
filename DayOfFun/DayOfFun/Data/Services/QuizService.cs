using DayOfFun.Data.Services.Contract;
using DayOfFun.Model;

namespace DayOfFun.Data.Services;

public class QuizService : IQuizService
{
    private readonly ApplicationDbContext _context;

    public QuizService(ApplicationDbContext context)
    {
        _context = context;
    }
    public void AddQuiz(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
        _context.SaveChanges();
    }
}