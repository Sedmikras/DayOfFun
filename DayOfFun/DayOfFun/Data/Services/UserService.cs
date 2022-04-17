using DayOfFun.Data.Services.Contract;
using DayOfFun.Model;
using Microsoft.EntityFrameworkCore;

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
        return _context.quizes.Where(quiz => quiz.Owner == user || quiz.Users.Contains(user)).ToList();
    }

    public User getUserByID(int userId)
    {
        return _context.users.Where(user => user.Id == userId).First();
    }

    public void addUser()
    {
        throw new NotImplementedException();
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