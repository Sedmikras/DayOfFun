using DayOfFun.Data.Services.Contracts;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.Data.Services;

public class QuizService : IQuizService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;

    private readonly ILogger _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(typeof(IQuizService));

    public QuizService(ApplicationDbContext context, IQuestionService questionService, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    /// <summary>
    /// Removes quiz from the database with check
    /// </summary>
    /// <param name="quizId">id of the quiz</param>
    public bool Delete(int quizId, User? u)
    {
        var quiz = _context.Quizzes.FirstOrDefault(q => q.Id == quizId);
        if (quiz == null) return false;
        _context.Quizzes.Remove(quiz);
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Returns quiz from the database and sets its info as output
    /// </summary>
    /// <param name="quizId"> ID of the quiz that will be read </param>
    /// <param name="quiz"> output object with properties from database</param>
    /// <returns> true if success </returns>
    public bool Read(int quizId, out Quiz quiz)
    {
        quiz = _context.Quizzes.FirstOrDefault(q => q.Id == quizId) ?? null;
        return quiz != null;
    }

    /// <summary>
    /// Returns simplified QuizViewModel for user - contains info about quiz so that it can be visualized
    /// </summary>
    /// <param name="session">Session with logged user</param>
    /// <param name="model">OUT QuizViewModel - simplified model for viewing quizzes</param>
    /// <returns>true if success / false if error</returns>
    public bool GetQuizzesModel(ISession session, out List<QuizViewModel> model)
    {
        if (_userService.GetUserFromSession(session, out var u)) return GetQuizzesModel(u, out model);
        _logger.LogError("Cannot read user from session, should redirect");
        model = new List<QuizViewModel>();
        return false;

    }

    /// <summary>
    /// Returns simplified QuizViewModel for user - contains info about quiz so that it can be visualized
    /// </summary>
    /// <param name="u">logged user</param>
    /// <param name="model">OUT QuizViewModel - simplified model for viewing quizzes</param>
    /// <returns>true if success / false if error</returns>
    public bool GetQuizzesModel(User u, out List<QuizViewModel> model)
    {
        model = u.Quizzes.Select(q => q.ToViewModel()).ToList();
        return true;
    }

    /// <summary>
    /// Validates model and updates its state in DB
    /// </summary>
    /// <param name="quiz">quiz</param>
    public void ValidateModel(Quiz quiz)
    {
        var answers = _context.Answers.Where(a => a.QuizId == quiz.Id).ToList();
        var numberOfAnswers = answers.Count;
        var numberOfQuestions = quiz.Questions.Count;
        var numberOfUsers = quiz.Users.Count;
        if (numberOfUsers == 0)
        {
            quiz.State = State.INVALID;
            return;
        }

        if (numberOfQuestions == 0)
        {
            quiz.State = State.CREATED;
            return;
        }

        quiz.State = numberOfAnswers == numberOfQuestions * numberOfUsers ? State.FINISHED : State.WAITING;
    }

    /// <summary>
    /// Returns simplified UserDetailsModel for user - contains info about users that participate in the quiz 
    /// </summary>
    /// <param name="quizId">id of the quiz</param>
    /// <param name="data">result - UserDetailsModel</param>
    /// <returns>true if success / false if error</returns>
    public bool ToShareUserViewModel(int quizId, out List<UserDetailsModel> data)
    {
        Quiz q;
        if (!Read(quizId, out q))
        {
            data = null!;
            _logger.LogError("Could not get model {Classname}", nameof(UserDetailsModel));
            return false;
        }

        var answers = _context.Answers.Where(a => a.Quiz.Id == quizId).ToList();
        var numberOfQuestions = q.Questions.Count;
        var results = (from user in q.Users
            let numberOfAnswers = answers.Count(a => a.User == user)
            let isResponded = (numberOfAnswers > 0)
            let isFinished = (numberOfAnswers == numberOfQuestions)
            select new UserDetailsModel()
            {
                Email = user.Email,
                Username = user.Username,
                IsFinished = isFinished,
                IsResponded = isResponded,
                QuizId = q.Id,
                NumberOfAnswers = numberOfAnswers,
                NumberOfQuestions = numberOfQuestions
            }).ToList();
        data = results;
        return true;
    }
}