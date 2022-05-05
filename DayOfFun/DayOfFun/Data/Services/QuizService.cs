using DayOfFun.Data.Services.Contract;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.Data.Services;

//TODO big rework here needed
public class QuizService : IQuizService
{
    private readonly ApplicationDbContext _context;
    private readonly IQuestionService _questionService;
    private readonly IUserService _userService;

    private readonly ILogger _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(typeof(IQuizService));

    public QuizService(ApplicationDbContext context, IQuestionService questionService, IUserService userService)
    {
        _context = context;
        _questionService = questionService;
        _userService = userService;
    } 

    public bool Add(Quiz quiz, User user)
    {
        throw new NotImplementedException();
    }

    public bool Update(Quiz quiz)
    {
        throw new NotImplementedException();
    }

    public bool Delete(int quizId, User? u)
    {
        throw new NotImplementedException();
    }

    public bool Delete(Quiz quiz)
    {
        throw new NotImplementedException();
    }

    public bool Read(int quizId, out Quiz quiz)
    {
        quiz = _context.Quizzes.FirstOrDefault(q => q.Id == quizId) ?? null;
        return quiz != null;
    }

    public bool GetQuizzesModel(ISession session, out List<QuizViewModel> model)
    {
        if (!_userService.GetUserFromSession(session, out var u))
        {
            _logger.LogError("Cannot read user from session, should redirect");
            model = new List<QuizViewModel>();
            return false;
        }

        model = u.Quizzes.Select(q => q.ToViewModel()).ToList();
        return true;
    }

    public bool ReadAllForUser(int userId, out List<Quiz> quizzes)
    {
        throw new NotImplementedException();
    }

    public void AddQuiz(Quiz quiz, ISession session)
    {
        throw new NotImplementedException();
    }

    public List<Quiz> getQuizzesForUser(User user)
    {
        throw new NotImplementedException();
    }

    public void Delete(ISession httpContextSession, int id)
    {
        throw new NotImplementedException();
    }

    public QuizAnswerModel getQuestionsFor(ISession httpContextSession, int id)
    {
        throw new NotImplementedException();
    }

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

    public Quiz getQuizById(int quizId)
    {
        throw new NotImplementedException();
    }

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