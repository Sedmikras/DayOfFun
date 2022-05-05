using System.Text;
using DayOfFun.Data.Services.Contract;
using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.Data.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    private static readonly ILogger Logger =
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(typeof(UserService));

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool RegisterUser(UserViewModel uvm)
    {
        if (_context.Users.FirstOrDefault(u => u.Email == uvm.Email) != null)
        {
            Logger.LogError("User with email {Email} already exists", uvm.Email);
            return false;
        }
        var user = uvm.ToApplicationUser();
        _context.Users.Add(user);
        _context.SaveChangesAsync();
        Logger.LogInformation("User with email {Email} successfully registered", uvm.Email);
        return true;
    }

    public User GetUserByID(int userId)
    {
        throw new NotImplementedException();
    }

    public void AddUser(User u)
    {
        _context.Users.Add(u);
        _context.SaveChanges();
    }

    public bool GetUserFromSession(ISession session, out User user)
    {
        if (session.IsAvailable && session.TryGetValue("UserId", out var value))
        {
            var userId = int.Parse(Encoding.Default.GetString(value));
            user = _context.Users.First(u => u.Id == userId);
            return true;
        }

        Logger.LogError("Could not get user from session");
        user = null!;
        return false;
    }

    public bool GetUserByEmail(string email, out User? user)
    {
        user = _context.Users.FirstOrDefault(u => u.Email == email);
        return user != null;
    }

    public User UpdateUser(int id, User newUser)
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public void AddTemporaryUser(string email)
    {
        AddUser(new User()
        {
            Email = email,
            IsTemporary = true
        });
    }
}