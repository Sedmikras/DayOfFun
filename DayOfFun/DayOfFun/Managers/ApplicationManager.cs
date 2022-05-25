using DayOfFun.Data;
using DayOfFun.Data.Services.Contracts;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.managers;

/// <summary>
/// Main logic class - sadly, without interface = chaotic development
/// </summary>
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
        _userService = userService;
        _questionService = questionService;
        _quizService = quizService;
        _context = context;
    }

    /// <summary>
    /// Returns quizzes for user
    /// </summary>
    /// <param name="session">Session with users credentials</param>
    /// <param name="model">Simplified view of quizzes</param>
    /// <returns>true if success / false if error</returns>
    public bool GetQuizzesForUser(ISession session, out List<QuizViewModel> model)
    {
        return _quizService.GetQuizzesModel(session, out model);
    }

    /// <summary>
    /// Returns quizzes for user
    /// </summary>
    /// <param name="u">instance of user</param>
    /// <param name="model">Simplified view of quizzes</param>
    /// <returns>true if success / false if error</returns>
    public bool GetQuizzesForUser(User u, out List<QuizViewModel> model)
    {
        return _quizService.GetQuizzesModel(u, out model);
    }

    /// <summary>
    /// Deletes quiz from application (DB)
    /// </summary>
    /// <param name="session">Session with users credentials</param>
    /// <param name="quizId">quiz id</param>
    /// <returns>true if success / false if error</returns>
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

    /// <summary>
    /// Creates quiz - inserts into DB
    /// </summary>
    /// <param name="session">Session with users credentials</param>
    /// <param name="quizModel">data from users input</param>
    /// <returns>true if success / false if error</returns>
    public bool CreateQuiz(ISession session, QuizCreateViewModel quizModel)
    {
        //var numberOfQuestion = quizModel.ViewCollection;
        if (!_userService.GetUserFromSession(session, out var user))
        {
            _logger.LogError("Couldn't read user from session");
            return false;
        }

        if (quizModel.Questions.Any(question => question.Text == null))
        {
            return false;
        }

        var questions = _questionService.GetQuestionsFromModel(quizModel);
        var q = new Quiz(user, quizModel, questions);

        if (q == null)
        {
            _logger.LogError("Couldn't create quiz");
            return false;
        }

        _context.Quizzes.Add(q);
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Prepares quiz to be filled - creates answers and propagate to user
    /// </summary>
    /// <param name="session">Session with users credentials</param>
    /// <param name="quizId">quiz ID</param>
    /// <param name="model">Fill model for quiz - questions with answers</param>
    /// <returns>true if success / false if error</returns>
    public bool GetQuizFillModel(ISession session, int quizId, out QuizAnswerModel? model)
    {
        if (_userService.GetUserFromSession(session, out var u)) return GetQuizFillModel(u, quizId, out model);
        model = null;
        _logger.LogError("Couldn't read user from session");
        return false;
    }

    /// <summary>
    /// Prepares quiz to be filled - creates answers and propagate to user
    /// </summary>
    /// <param name="user">user instance</param>
    /// <param name="quizId">quiz ID</param>
    /// <param name="model">Fill model for quiz - questions with answers</param>
    /// <returns>true if success / false if error</returns>
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

    /// <summary>
    /// Updates quiz and validates it (change is propagated to the DB)
    /// </summary>
    /// <param name="u">user instance</param>
    /// <param name="model">user input model</param>
    /// <returns>State of quiz - if error = STATE.INVALID</returns>
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

    /// <summary>
    /// gets user from session and calls same method with user as parameter
    /// </summary>
    /// <param name="session">user credentials in session</param>
    /// <param name="model">user input model</param>
    /// <returns>State of quiz - if error = STATE.INVALID</returns>
    public State UpdateQuiz(ISession session, QuizAnswerModel model)
    {
        if (_userService.GetUserFromSession(session, out var u)) return UpdateQuiz(u, model);
        _logger.LogError("Couldn't get user from session");
        return State.INVALID;
    }

    /// <summary>
    /// prepares details of quiz from db and returns them in model
    /// </summary>
    /// <param name="u">user instance</param>
    /// <param name="quizId">id of the quiz</param>
    /// <param name="model">details about quiz model</param>
    /// <returns>true if success / false if error</returns>
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

    /// <summary>
    /// prepares details of quiz from db and returns them in model
    /// </summary>
    /// <param name="session">user credentials in session</param>
    /// <param name="quizId">id of the quiz</param>
    /// <param name="model">details about quiz model</param>
    /// <returns>true if success / false if error</returns>
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

    /// <summary>
    /// Temporary login for users coming from public access. 
    /// </summary>
    /// <param name="email">email of the user</param>
    /// <param name="user">OUT user - instance from DB</param>
    /// <returns>true if success / false if error</returns>
    public bool TemporaryLogin(string email, out User user)
    {
        return _userService.GetUserByEmail(email, out user);
    }

    

    /// <summary>
    /// Async suggest question text when filling quiz
    /// </summary>
    /// <param name="session">user credentials in session</param>
    /// <param name="term">start of the text </param>
    /// <returns>List of question texts</returns>
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

    /// <summary>
    /// returns info about users concerned in given quiz
    /// </summary>
    /// <param name="session">user credentials in session</param>
    /// <param name="quizId">quiz id</param>
    /// <param name="model">output - model with users info</param>
    /// <returns>true if success / false if error</returns>
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
    
    /// <summary>
    /// Validates email - if user with given email is in DB
    /// </summary>
    /// <param name="email">email of the user (public access)</param>
    /// <param name="q">quiz that will be added to given user</param>
    /// <param name="user">output instance of user</param>
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

    /// <summary>
    /// shares quiz with user given in model (contains user email)
    /// </summary>
    /// <param name="session">user credentials in session</param>
    /// <param name="suv">model</param>
    /// <returns>true if success / false if error</returns>
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

    /// <summary>
    /// Removes user from quiz so the user wont be participating
    /// </summary>
    /// <param name="email">email of the user</param>
    /// <param name="quizId">id of the quiz</param>
    /// <returns>true if success / false if error</returns>
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

    /// <summary>
    /// Adds question to the quiz
    /// </summary>
    /// <param name="user">user that is performing this action</param>
    /// <param name="question">question that will be added</param>
    /// <param name="quizId">id of the quiz that will be updated</param>
    /// <returns>true if success / false if error</returns>
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

    /// <summary>
    /// Adds question to the quiz
    /// </summary>
    /// <param name="user">user credentials in session</param>
    /// <param name="question">question that will be added</param>
    /// <param name="quizId">id of the quiz that will be updated</param>
    /// <returns>true if success / false if error</returns>
    public bool AddQuestion(ISession session, Question question, int quizId)
    {
        return _userService.GetUserFromSession(session, out var current) && AddQuestion(current, question, quizId);
    }
}