using System.Text;
using DayOfFun.Data.Services.Contracts;
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

    /// <summary>
    /// registers user - save user to the database
    /// </summary>
    /// <param name="uvm">info about users - email (unique) / password / username</param>
    /// <returns>true if success / false if error</returns>
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

    /// <summary>
    /// Return user from DB given information in session (logged user)
    /// </summary>
    /// <param name="session">Session with info about users</param>
    /// <param name="user">User instance from DB given the info from session</param>
    /// <returns>true if success / false if error</returns>
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

    /// <summary>
    /// Return user from DB given email (temporary login from public access) 
    /// </summary>
    /// <param name="email">email of the public (non-registered, non-logged) user</param>
    /// <param name="u">User instance from DB given the info from session</param>
    /// <returns>true if success / false if error</returns>
    public bool GetUserByEmail(string email, out User? user)
    {
        user = _context.Users.FirstOrDefault(u => u.Email == email);
        return user != null;
    }

    
    /*
    public void AddTemporaryUser(string email)
    {
        AddUser(new User()
        {
            Email = email,
            IsTemporary = true
        });
    }*/


    /*private void AddUser(User u)
    {
        _context.Users.Add(u);
        _context.SaveChanges();
    }*/
}