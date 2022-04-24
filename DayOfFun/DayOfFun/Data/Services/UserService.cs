using DayOfFun.Data.Services.Contract;
using DayOfFun.Model;
namespace DayOfFun.Data.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

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

    public User getUserByID(int userId)
    {
        return _context.Users.Where(user => user.Id == userId).First();
    }

    public void addUser()
    {
        throw new NotImplementedException();
    }

    public User getUserFromSession(ISession Session)
    {
        if (Session.IsAvailable)
        {
            var userId =  Int32.Parse(Session.GetString("UserId"));
            User u = _context.Users.Where(user => user.Id == userId).First();
            return u;
        }
        return null;
    }

    public User updateUser(int id, User newUser)
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }
}