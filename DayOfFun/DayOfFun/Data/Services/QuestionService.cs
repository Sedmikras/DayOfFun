using DayOfFun.Data.Services.Contract;
using DayOfFun.Models.DB;

namespace DayOfFun.Data.Services;

public class QuestionService : IQuestionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuestionService> _logger;
    
    public QuestionService(ApplicationDbContext context, ILogger<QuestionService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public void AddQuestionForQuiz(Question question, Quiz quiz)
    {
        throw new NotImplementedException();
    }

    public bool AddQuestionsForQuiz(Quiz quiz)
    {
        throw new NotImplementedException();
    }

    public List<Question> getQuestionsForQuizAndUser(Quiz quiz, User user)
    {
        throw new NotImplementedException();
    }

    public void removeDuplicities(Quiz quiz, out List<Question> questions)
    {
        throw new NotImplementedException();
    }
}