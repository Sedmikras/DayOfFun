using DayOfFun.Data.Services.Contract;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;

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

    public bool AddQuestionForQuiz(Question question, Quiz quiz)
    {
        var result = _context.Questions.FirstOrDefault(q => q.Text == question.Text);
        if (result == null) result = question;
        quiz.Questions.Add(result);
        return true;
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

    public List<Question> getQuestionsFromModel(QuizCreateViewModel quizModel)
    {
        var questions = new List<Question>();
        foreach (var question in quizModel.Questions)
        {
            var q = _context.Questions.FirstOrDefault(q => q.Text == question.Text);
            if (q != null)
            {
                questions.Add(q);
            }
        }

        return questions;
    }
}