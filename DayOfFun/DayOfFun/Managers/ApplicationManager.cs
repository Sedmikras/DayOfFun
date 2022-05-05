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
        if (_quizService.GetQuizzesModel(session, out model))
            return true;
        return false;
    }

    public bool DeleteQuiz(ISession session, int quizId)
    {
        User u;
        if (!_userService.GetUserFromSession(session, out u))
        {
            return false;
            throw new Exception();
        }

        _quizService.Delete(quizId, u);
        return true;
    }

    public bool CreateQuiz(ISession session, QuizCreateViewModel quizModel)
    {
        User user;
        //var numberOfQuestion = quizModel.ViewCollection;
        if (!_userService.GetUserFromSession(session, out user))
        {
            //TODO
            throw new Exception();
        }

        Quiz q = new Quiz(user, quizModel);
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

    public QuizAnswerModel GetQuizFillModel(ISession session, int quizId, List<Answer> answers)
    {
        User user;
        if (!_userService.GetUserFromSession(session, out user))
        {
            throw new Exception();
        }

        Quiz quiz = _context.Quizzes.Where(q => q.Id == quizId).FirstOrDefault();
        if (quiz == null)
        {
            throw new Exception();
        }

        return quiz.ToAnswerModel(user, answers);
    }

    public QuizAnswerModel GetQuizFillModel(ISession session, int quizId)
    {
        User u;
        if (!_userService.GetUserFromSession(session, out u))
        {
            throw new Exception();
        }

        return GetQuizFillModel(u, quizId);
    }

    public QuizAnswerModel GetQuizFillModel(User user, int quizId)
    {
        var quiz = _context.Quizzes.FirstOrDefault(q => q.Id == quizId);
        var answers = _context.Answers.Where(a => a.QuizId == quizId && a.UserId == user.Id).ToList();
        if (quiz == null)
        {
            throw new Exception();
        }

        return quiz.ToAnswerModel(user, answers);
    }

    public State ValidateQuiz(Quiz quiz)
    {
        return State.CREATED;
        throw new NotImplementedException();
        return State.INVALID;
    }

    public State UpdateQuiz(User u, QuizAnswerModel model)
    {
        var quizId = model.QuizId;
        Quiz quiz;
        if (!_quizService.Read(quizId, out quiz))
        {
            throw new Exception();
        }

        if (quiz == null)
        {
            throw new Exception();
        }

        IEnumerable<Answer> answers = quiz.AddAnswers(model.QuestionAnswers, u);
        foreach (var answer in answers)
        {
            var contextAnswer = _context.Answers.FirstOrDefault(a => a.QuizId == quiz.Id && a.QuestionId == answer.QuestionId && a.UserId == u.Id);
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
            _context.SaveChangesAsync();
            return quiz.State;
        }
        else
        {
            return State.INVALID;
        }
    }

    public State UpdateQuiz(ISession session, QuizAnswerModel model)
    {
        User u;
        if (!_userService.GetUserFromSession(session, out u))
        {
            throw new Exception();
        }

        return UpdateQuiz(u, model);
    }

    public QuizDetailsModel GetQuizDetailsModel(ISession session, int quizId)
    {
        User u;
        if (!_userService.GetUserFromSession(session, out u))
        {
            throw new Exception();
        }

        Quiz quiz;
        if (!_quizService.Read(quizId, out quiz))
        {
            throw new Exception();
        }

        List<Answer> answers = _context.Answers.Where(a => a.QuizId == quiz.Id).ToList();

        return new QuizDetailsModel(quiz, answers);
    }

    public bool TemporaryLogin(string email, out User user)
    {
        return _userService.GetUserByEmail(email, out user);
    }

    public void Share(ISession session, string email)
    {
        User u;
        if (!_userService.GetUserFromSession(session, out u))
        {
            throw new Exception();
        }

        User? newUser;
        if (_userService.GetUserByEmail(email, out newUser))
        {
        }
        else
        {
            _userService.AddTemporaryUser(email);
        }
    }

    public async Task<List<string>> SuggestQuestionsAsync(ISession session, string term)
    {
        User u;
        if (!_userService.GetUserFromSession(session, out u))
        {
            throw new Exception();
        }

        var questionsText = _context.Questions.Where(q => q.Text.Contains(term)).Select(q => q.Text).ToList();
        return questionsText;
    }

    //TODO - asi bude potŘeba to trochu změnit
    public List<UserDetailsModel> getQuizUsersView(ISession session, int quizId)
    {
        User u;
        Quiz q;
        List<UserDetailsModel> data;
        if (!_userService.GetUserFromSession(session, out u) || !_quizService.ToShareUserViewModel(quizId, out data))
        {
            throw new Exception();
        }

        return data != null ? data : new List<UserDetailsModel>();
    }

    /*public async Task<IActionResult> ValidateEmail(HttpContext httpContext, ShareUserViewModel suv)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await _context.Users.Where(suv.Email == suv.Email).Any()
    }*/
    public void ValidateEmail(string email, Quiz q, out User? user)
    {
        User u = _context.Users.FirstOrDefault(u => u.Email == email);
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
        User current;
        User added;
        Quiz q;
        if (!_userService.GetUserFromSession(session, out current) || !_quizService.Read(suv.QuizId, out q))
        {
            throw new Exception();
        }
        ValidateEmail(suv.Email, q, out added);
        current.Quizzes.Add(q);
        _quizService.ValidateModel(q);
        _context.SaveChanges();
        return true;
    }

    public bool RemoveUser(string email, int quizId)
    {
        User u;
        Quiz q;
        if (!_userService.GetUserByEmail(email, out u) || !_quizService.Read(quizId, out q))
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
}