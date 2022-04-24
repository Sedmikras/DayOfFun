using DayOfFun.Model;

namespace DayOfFun.Data.Services.Contract;

public interface IUserService
{
    IEnumerable<Quiz> getQuizzesByUserId(int userId);

    User getUserByID(int userId);

    void addUser();

    public User getUserFromSession(ISession session);

    User updateUser(int id, User newUser);

    void Delete();
}