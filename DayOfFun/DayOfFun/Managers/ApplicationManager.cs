using DayOfFun.Data;
using DayOfFun.Data.Services.Contract;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.managers;

public class ApplicationManager
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;

    private readonly ILogger _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(typeof(ApplicationManager));

    public ApplicationManager(IUserService userService, IQuizService quizService, IQuestionService questionService,
        ApplicationDbContext context)
    {
        this._userService = userService;
        this._questionService = questionService;
        this._quizService = quizService;
        this._context = context;
    }

    public bool GetQuizzesForUser(ISession session, out List<QuizViewModel> model)
    {
        return _quizService.GetQuizzesModel(session, out model);
    }

    public bool GetQuizzesForUser(User u, out List<QuizViewModel> model)
    {
        return _quizService.GetQuizzesModel(u, out model);
    }

    public bool DeleteQuiz(ISession session, int quizId)
    {
        if (!_userService.GetUserFromSession(session, out var u))
        {
            _logger.LogError("Couldn't read user from session");
            return false;
        }

        _quizService.Delete(quizId, u);
        return true;
    }

    public bool CreateQuiz(ISession session, QuizCreateViewModel quizModel)
    {
        //var numberOfQuestion = quizModel.ViewCollection;
        if (!_userService.GetUserFromSession(session, out var user))
        {
            //TODO
            throw new Exception();
        }

        foreach (var question in quizModel.Questions)
        {
            if (question.Text == null)
            {
                return false;
            }
        }

        var questions = _questionService.getQuestionsFromModel(quizModel);
        var q = new Quiz(user, quizModel, questions);

        if (q == null)
        {
            //TODO
            throw new Exception();
        }

        _context.Quizzes.Add(q);
        _context.SaveChanges();

        /*if (ValidateQuiz(quizModel) == State.INVALID)
        {
            //TODO
            throw new Exception();
        }
        */

        return true;
    }

    public bool GetQuizFillModel(ISession session, int quizId, List<Answer> answers, out QuizAnswerModel? model)
    {
        if (!_userService.GetUserFromSession(session, out var user))
        {
            _logger.LogError("Couldn't read user from session");
            model = null;
            return false;
        }

        var quiz = _context.Quizzes.FirstOrDefault(q => q.Id == quizId);
        if (quiz != null)
        {
            model = quiz.ToAnswerModel(user, answers);
            return true;
        }

        _logger.LogError("Couldn't find quiz with id: {QuizId}", quizId);
        model = null;
        return false;
    }

    public bool GetQuizFillModel(ISession session, int quizId, out QuizAnswerModel? model)
    {
        if (_userService.GetUserFromSession(session, out var u)) return GetQuizFillModel(u, quizId, out model);
        model = null;
        _logger.LogError("Couldn't read user from session");
        return false;
    }

    public bool GetQuizFillModel(User user, int quizId, out QuizAnswerModel? model)
    {
        var quiz = _context.Quizzes.FirstOrDefault(q => q.Id == quizId);
        var answers = _context.Answers.Where(a => a.QuizId == quizId && a.UserId == user.Id).ToList();

        if (quiz == null)
        {
            _logger.LogError("Couldn't get quiz wit id {QuizID}", quizId);
            model = null;
            return false;
        }

        model = quiz.ToAnswerModel(user, answers);
        return true;
    }

    public State UpdateQuiz(User u, QuizAnswerModel model)
    {
        var quizId = model.QuizId;
        if (!_quizService.Read(quizId, out var quiz))
        {
            _logger.LogError("Couldn't get quiz wit id {QuizID}", quizId);
            return State.INVALID;
        }

        var answers = quiz.AddAnswers(model.QuestionAnswers, u);
        foreach (var answer in answers)
        {
            var contextAnswer = _context.Answers.FirstOrDefault(a =>
                a.QuizId == quiz.Id && a.QuestionId == answer.QuestionId && a.UserId == u.Id);
            if (contextAnswer != null)
            {
                contextAnswer.Result = answer.Result;
                _context.Answers.Update(contextAnswer);
            }
            else
            {
                _context.Answers.Add(answer);
            }
        }

        _context.SaveChanges();

        //validate quiz state
        _quizService.ValidateModel(quiz);
        if (quiz.State != State.INVALID)
        {
            _context.Quizzes.Update(quiz);
            _context.SaveChanges();
            return quiz.State;
        }
        else
        {
            return State.INVALID;
        }
    }

    public State UpdateQuiz(ISession session, QuizAnswerModel model)
    {
        if (_userService.GetUserFromSession(session, out var u)) return UpdateQuiz(u, model);
        _logger.LogError("Couldn't get user from session");
        return State.INVALID;
    }

    public bool GetQuizDetailsModel(User u, int quizId, out QuizDetailsModel? model)
    {
        if (!_quizService.Read(quizId, out var quiz))
        {
            _logger.LogError("Couldn't get quiz wit id {QuizID}", quizId);
            model = null;
            return false;
        }

        var answers = _context.Answers.Where(a => a.QuizId == quiz.Id).ToList();
        model = new QuizDetailsModel(quiz, answers);
        return true;
    }

    public bool GetQuizDetailsModel(ISession session, int quizId, out QuizDetailsModel? model)
    {
        if (!_userService.GetUserFromSession(session, out _))
        {
            _logger.LogError("Couldn't get user from session");
            model = null;
            return false;
        }

        if (!_quizService.Read(quizId, out var quiz))
        {
            _logger.LogError("Couldn't get quiz wit id {QuizID}", quizId);
            model = null;
            return false;
        }

        var answers = _context.Answers.Where(a => a.QuizId == quiz.Id).ToList();
        model = new QuizDetailsModel(quiz, answers);
        return true;
    }

    public bool TemporaryLogin(string email, out User user)
    {
        return _userService.GetUserByEmail(email, out user);
    }

    public void Share(ISession session, string email)
    {
        if (!_userService.GetUserFromSession(session, out _))
        {
            _logger.LogError("Couldn't get user from session");
            return;
        }

        if (_userService.GetUserByEmail(email, out _))
        {
        }
        else
        {
            _userService.AddTemporaryUser(email);
        }
    }

    public async Task<List<string>> SuggestQuestionsAsync(ISession session, string term)
    {
        if (!_userService.GetUserFromSession(session, out _))
        {
            return null;
        }

        return await Task.Run(() =>
        {
            var questionsText = _context.Questions.Where(q => q.Text.Contains(term)).Select(q => q.Text).ToList();
            return questionsText;
        });
    }

    //TODO - asi bude potŘeba to trochu změnit
    public bool GetQuizUsersView(ISession session, int quizId, out List<UserDetailsModel>? model)
    {
        Quiz q;
        if (!_userService.GetUserFromSession(session, out _) ||
            !_quizService.ToShareUserViewModel(quizId, out var data))
        {
            _logger.LogError(
                "Couldn't get user details model. Either session is null or quiz with ID {QuizId} does not exists",
                quizId);
            model = null;
            return false;
        }

        model = data != null ? data : new List<UserDetailsModel>();
        return true;
    }

    /*public async Task<IActionResult> ValidateEmail(HttpContext httpContext, ShareUserViewModel suv)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await _context.Users.Where(suv.Email == suv.Email).Any()
    }*/
    private void ValidateEmail(string email, Quiz q, out User? user)
    {
        var u = _context.Users.FirstOrDefault(u => u.Email == email);
        if (u == null)
        {
            u = new User()
            {
                Email = email,
                IsTemporary = true
            };
            u.Quizzes.Add(q);
            _context.Users.AddAsync(u);
        }

        u.Quizzes.Add(q);
        user = u;
    }

    public bool ShareQuiz(ISession session, ShareUserViewModel suv)
    {
        if (!_userService.GetUserFromSession(session, out var current) || !_quizService.Read(suv.QuizId, out var q))
        {
            _logger.LogError("Couldn't get user from session");
            return false;
        }

        ValidateEmail(suv.Email, q, out var addedUser);
        current.Quizzes.Add(q);
        q.Users.Add(addedUser);
        _quizService.ValidateModel(q);
        _context.Quizzes.Update(q);
        _context.SaveChanges();
        return true;
    }

    public bool RemoveUser(string email, int quizId)
    {
        if (!_userService.GetUserByEmail(email, out var u) || !_quizService.Read(quizId, out var q))
        {
            return false;
        }

        q.Users.Remove(u);
        u.Quizzes.Remove(q);
        _context.Answers.RemoveRange(_context.Answers.Where(a => a.QuizId == quizId && a.UserId == u.Id).ToList());
        _quizService.ValidateModel(q);
        _context.SaveChanges();
        return true;
    }

    public bool AddQuestion(User user, Question question, int quizId)
    {
        if(!_quizService.Read(quizId, out var q) || !_questionService.AddQuestionForQuiz(question, q))
        {
            _logger.LogError("Couldn't get user from session");
            return false;
        }
        _quizService.ValidateModel(q);
        _context.Quizzes.Update(q);
        _context.SaveChanges();
        return true;
    }

    public bool AddQuestion(ISession session, Question question, int quizId)
    {
        return _userService.GetUserFromSession(session, out var current) && AddQuestion(current, question, quizId);
    }
}