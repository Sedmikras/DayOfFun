using DayOfFun.Data.Services.Contracts;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.Data.Services;

public class QuestionService : IQuestionService
{
    private readonly ApplicationDbContext _context;

    public QuestionService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Add question to the quiz
    /// </summary>
    /// <param name="question">question</param>
    /// <param name="quiz">quiz</param>
    /// <returns>true if success / false if not successful</returns>
    public bool AddQuestionForQuiz(Question question, Quiz quiz)
    {
        var result = _context.Questions.FirstOrDefault(q => q.Text == question.Text) ?? question;
        quiz.Questions.Add(result);
        return true;
    }

    /// <summary>
    /// returns list of questions from the DB where questions text is given in parameter (model)
    /// </summary>
    /// <param name="quizModel">list of questions from users input</param>
    /// <returns>List of questions that are in DB</returns>
    public List<Question> GetQuestionsFromModel(QuizCreateViewModel quizModel)
    {
        return quizModel.Questions.Select(question => _context.Questions.FirstOrDefault(q => q.Text == question.Text)).Where(q => q != null).ToList();
    }
}