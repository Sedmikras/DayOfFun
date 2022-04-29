using DayOfFun.Data.Services.Contract;
using DayOfFun.Models;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.Data.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    private static ILogger _logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(typeof(UserService));

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool RegisterUser(UserViewModel uvm, out User user)
    {
        if (_context.Users.Where(u => u.Email == uvm.Email).FirstOrDefault() != null)
        {
            user = null;
            _logger.LogError("User with email {Email} already exists", uvm.Email);
            return false;
        }

        user = uvm.ToApplicationUser();
        _context.Users.Add(user);
        _context.SaveChangesAsync();
        _logger.LogInformation("User with email {Email} successfully registered", uvm.Email);
        return true;
    }

/*
    public IEnumerable<Quiz> getQuizzesByUserId(int userId)
    {
        User user = getUserByID(userId);
        List<Quiz> quizzes = new List<Quiz>();
        foreach (var quizzesUser in user.Quizzes_Users)
        {
            quizzes.Add(quizzesUser.quiz);
        }
        return quizzes;
    }

    public bool getQuizzIdsForUser(ISession session, out List<Quizzes_Users> quizzesUsers)
    {
        User u;
        if (!getUserFromSession(session, out u))
        {
            quizzesUsers = null;
            return false;
        }
        quizzesUsers = _context.Quizzes_Users.Where(qu => qu.userId == u.Id).ToList();
        return true;
    }

    public User getUserByID(int userId)
    {
        return _context.Users.Where(user => user.Id == userId).First();
    }

    public void addUser()
    {
        throw new NotImplementedException();
    }

    public bool getUserFromSession(ISession session, out User user)
    {
        if (session.IsAvailable)
        {
            var userId =  Int32.Parse(session.GetString("UserId"));
            user = _context.Users.Where(user => user.Id == userId).First();
            return true;
        }
        user = null;
        return false;
    }

    public User updateUser(int id, User newUser)
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }*/

    public User getUserByID(int userId)
    {
        throw new NotImplementedException();
    }

    public void addUser()
    {
        throw new NotImplementedException();
    }

    public bool getUserFromSession(ISession session, out User user)
    {
        if (session.IsAvailable)
        {
            var userId = Int32.Parse(session.GetString("UserId"));
            user = _context.Users.Where(user => user.Id == userId).First();
            return true;
        }

        user = null;
        return false;
    }

    public bool GetUserByEmail(string email, out User user)
    {
        user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public User updateUser(int id, User newUser)
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public void AddTemporaryUser(string email)
    {
        User newUser = new User()
        {
            Email = email,
            IsTemporary = true
        };
        _context.Users.Add(newUser);
        _context.SaveChanges();
    }
}