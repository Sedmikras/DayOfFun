using Castle.Core;
using DayOfFun.BeginCollectionItemCore.Models;
using DayOfFun.Data.Services.Contract;
using DayOfFun.Model;
using DayOfFun.Models.Domain;

namespace DayOfFun.Data.Services;

//TODO big rework here needed
public class QuizService : IQuizService
{
    private readonly ApplicationDbContext _context;
    private readonly IQuestionService _questionService;
    private readonly IUserService _userService;

    public static ILogger _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(typeof(IQuizService));

    public QuizService(ApplicationDbContext context, IQuestionService questionService, IUserService userService)
    {
        _context = context;
        _questionService = questionService;
        _userService = userService;
    } /*

    public bool Add(Quiz quiz, User user)
    {
        if (quiz.State != State.CREATED)
        {
            _logger.LogError("Quiz with name {Title}, owner {Owner}, Is not in valid state", quiz.Title, quiz.OwnerId);
            return false;
        }

        quiz.Owner = user;
        quiz.OwnerId = user.Id;
        //TODO check duplicates ?
        _context.Quizzes.Add(quiz);
        _context.SaveChanges();
        _context.Quizzes_Users.Add(new Quizzes_Users()
        {
            quizId = quiz.Id,
            userId = user.Id
        });
        return true;
    }

    public bool Update(Quiz quiz)
    {
        throw new NotImplementedException();
    }

    public bool Delete(int quizId, User u)
    {
        Quiz quiz;
        Read(quizId, out quiz);
        /*if (!quiz.isValid())
        {
            return false;
        }#1#

        List<Quizzes_Users> qus = _context.Quizzes_Users.Where(qu => qu.userId == u.Id).ToList();
        foreach (var qu in qus)
        {
            _context.Question_Users.Remove(qu);
        }

        List<Quizzes_Quesitons> qqs = _context.Quizzes_Questions.Where(qq => qq.QuizID == quizId).ToList();
        foreach (var qq in qqs)
        {
            _context.Quizzes_Questions.Remove(qq);
        }

        _context.Quizzes.Remove(_context.Quizzes.Find(quiz.Id));
        _context.SaveChangesAsync();
        return true;
    }

    public bool Delete(Quiz quiz)
    {
        throw new NotImplementedException();
    }

    public bool Read(int quizId, out Quiz quiz)
    {
        quiz = _context.Quizzes.Where(q => q.Id == quizId).FirstOrDefault();
        List<Quizzes_Users> qus = _context.Question_Users.Where(qu => qu.quizId == quizId).ToList();
        HashSet<User> users = new HashSet<User>();
        foreach (var qu in qus)
        {
            users.Add(_context.Users.Where(u => u.Id == qu.userId).FirstOrDefault());
        }
        List<Quizzes_Quesitons> qqs = _context.Quizzes_Questions.Where(qqs => qqs.QuizID == quizId).ToList();
        List<Question> questions = new List<Question>();
        foreach (var qq in qqs)
        {
               questions.Add(_context.Questions.Where(q => q.Id == qq.quesitonId).FirstOrDefault());
        }
        quiz.Users = users;
        quiz.Questions = questions;
        return true;
    }

    public bool ReadAllForUser(int userId, out List<Quiz> quizzes)
    {
        throw new NotImplementedException();
    }

    public void AddQuiz(Quiz quiz, ISession Session)
    {
        User u = _context.Users.Where(u => u.Id == Int32.Parse(Session.GetString("UserId"))).First();
        quiz.Owner = u;
        quiz.OwnerId = u.Id;
        _context.Quizzes.Add(quiz);
        quiz.Users.Add(u);
        _context.SaveChanges();
        _context.Quizzes_Users.Add(new Quizzes_Users() {userId = u.Id, user = u, quizId = quiz.Id, quiz = quiz});
        List<Question> withoutDuplicities;
        _questionService.removeDuplicities(quiz, out withoutDuplicities);
        foreach (var question in withoutDuplicities)
        {
            _questionService.AddQuestionForQuiz(question, quiz);
        }

        _context.SaveChanges();
        State state = ValidateState(quiz);
        if (quiz.State != state)
        {
            _context.SaveChanges();
        }
    }

    public List<Quiz> getQuizzesForUser(User user)
    {
        List<Quizzes_Users> quizIds = _context.Quizzes_Users.Where(qu => qu.userId == user.Id).ToList();
        List<int> ids = new List<int>();
        foreach (var Quizzes_Users in quizIds)
        {
            ids.Add(Quizzes_Users.quizId);
        }

        List<Quiz> quizzes = _context.Quizzes.Where(q => ids.Contains(q.Id)).ToList();
        return quizzes;
    }

    public void Delete(ISession session, int id)
    {
        Quiz quiz = _context.Quizzes.Where(quiz => quiz.Id == id).First();
        if (quiz.State == State.CREATED)
        {
            _context.Quizzes.Remove(quiz);
            Quizzes_Users qu = _context.Quizzes_Users.Where(qu => qu.quizId == id).First();
            _context.Quizzes_Users.Remove(qu);
            _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception();
        }
    }

    public Quiz_Answer_Model getQuestionsFor(ISession session, int id)
    {
        User u;
        _userService.getUserFromSession(session, out u);
        Quiz quiz = _context.Quizzes.Where(quiz => quiz.Id == id).First();
        List<Question> questions = _questionService.getQuestionsForQuizAndUser(quiz, u);
        quiz.Questions.AddRange(questions);
        Quiz_Answer_Model qam = new Quiz_Answer_Model(questions, u.Id, quiz.Id, quiz.Title);
        return qam;
    }

    public void ValidateModel(Quiz_Answer_Model model)
    {
        User currentUser = _userService.getUserByID(model.UserId);
        //save answers
        foreach (var ans in model.QuestionAnswers)
        {
            if (_context.Answers.Where(a =>
                    a.QuestionId == ans.QuestionId && a.QuizId == ans.QuizId && a.UserId == ans.UserId).Any())
            {
                _context.Answers.Update(ans);
            }
            else
            {
                _context.Answers.Add(ans);
            }
        }

        _context.SaveChanges();
        Quiz update;
        if (ValidateQuiz(model, currentUser, out update))
        {
            _context.Quizzes.Add(update);
        }

        Quiz quiz = _context.Quizzes.Where(q => q.Id == model.QuizId).First();
        //_quizManager.resolveChanges(quiz);
        _context.SaveChanges();
        return;
        //throw new NotImplementedException();
    }

    public Quiz getQuizById(int quizId)
    {
        throw new NotImplementedException();
        return null;
    }

    public bool getQuizById(int quizId, ISession session, out Quiz quiz)
    {
        {
            quiz = _context.Quizzes.Where(q => q.Id == quizId).First();
            User u;
            _userService.getUserFromSession(session, out u);
            List<Question> questions = _questionService.getQuestionsForQuizAndUser(quiz, u);
            quiz.Questions = questions;
            return true;
        }
    }

    private bool ValidateQuiz(Quiz_Answer_Model model, User user, out Quiz quizResult)
    {
        Quiz quiz = _context.Quizzes.Where(q => q.Id == model.QuizId).First();
        switch (quiz.State)
        {
            // without questions
            case State.CREATED:
            {
                addAllQuestions(quiz, model.Questions);
            }
                break;
            // waiting for users
            case State.WAITING:
            {
                addAllQuestions(quiz, model.Questions);
                updateAnswers(quiz, model.QuestionAnswers);
            }
                break;
            // waiting for filling
            case State.PREPARED:
            {
                updateAnswers(quiz, model.QuestionAnswers);
            }
                break;
            // check results
            case State.FINISHED: break;
        }

        quizResult = new Quiz();
        return false;
    }

    private void addAllQuestions(Quiz quiz, List<Question> questions)
    {
        if (quiz.Questions.Count != 0)
        {
            throw new Exception();
        }

        foreach (var question in questions)
        {
            var dbQuestion = _context.Questions.Where(q => q.Text == question.Text).First();
            int quesitonId;
            if (dbQuestion != null)
            {
                quesitonId = dbQuestion.Id;
                quiz.Questions.Add(dbQuestion);
            }
            else
            {
                question.Quizzes.Add(quiz);
                _context.Questions.Add(question);
                _context.SaveChanges();
                quesitonId = question.Id;
                quiz.Questions.Add(question);
            }

            _context.Quizzes_Questions.Add(new Quizzes_Quesitons() {QuizID = quiz.Id, quesitonId = quesitonId});
        }

        _context.SaveChanges();

        quiz.Questions = questions;
    }

    private void updateAnswers(Quiz quiz, List<Answer> answers)
    {
        if (quiz.Questions.Count != 0)
        {
            throw new Exception();
        }

        foreach (var answer in answers)
        {
            var dbAnswer = _context.Answers.Where(a =>
                    a.QuizId == answer.QuizId && a.UserId == answer.UserId && a.QuestionId == answer.QuestionId)
                .First();
            if (dbAnswer != null)
            {
                dbAnswer.Result = answer.Result;
                _context.Answers.Update(dbAnswer);
            }
            else
            {
                _context.Answers.Add(answer);
            }
        }

        quiz.State = ValidateState(quiz);
    }

    private State ValidateState(Quiz quiz)
    {
        var users = _context.Quizzes_Users.Where(qu => qu.quizId == quiz.Id).ToList();
        var questions = _context.Quizzes_Questions.Where(qq => qq.QuizID == quiz.Id).ToList();
        var quizAnswers = _context.Answers.Where(answer => answer.QuizId == quiz.Id).ToList();

        if (users.Count == 1 && questions.Count == 0)
        {
            return State.CREATED;
        }

        if (users.Count == 1 && questions.Count != 0 && quizAnswers.Count == 0)
        {
            return State.PREPARED;
        }

        if (quizAnswers.Count == users.Count * questions.Count)
        {
            return State.FINISHED;
        }
        else
        {
            return State.WAITING;
        }

        return State.INVALID;
    }*/

    public bool Add(Quiz quiz, User user)
    {
        throw new NotImplementedException();
    }

    public bool Update(Quiz quiz)
    {
        throw new NotImplementedException();
    }

    public bool Delete(int quizId, User u)
    {
        throw new NotImplementedException();
    }

    public bool Delete(Quiz quiz)
    {
        throw new NotImplementedException();
    }

    public bool Read(int quizId, out Quiz quiz)
    {
        quiz = _context.Quizzes.Where(q => q.Id == quizId).FirstOrDefault();
        if (quiz == null)
            return false;
        return true;
    }

    public List<QuizViewModel> GetQuizzesModel(ISession session)
    {
        
        User u = _context.Users.Where(u => u.Id == Int32.Parse(session.GetString("UserId"))).FirstOrDefault();
        if (u == null)
        {
            _logger.LogError("Couldn't read user from database");
            return null;
        }

        List<QuizViewModel> qvm = new List<QuizViewModel>();
        foreach (var q in u.Quizzes)
        {
            qvm.Add(q.ToViewModel());
        }

        return qvm;
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
        List<Answer> answers = _context.Answers.Where(a => a.QuizId == quiz.Id).ToList();
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
        return;
    }

    public Quiz getQuizById(int quizId)
    {
        throw new NotImplementedException();
    }
}