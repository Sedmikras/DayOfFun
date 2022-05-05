using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.Data.Services.Contract;

public interface IUserService
{
    bool RegisterUser(UserViewModel uvm);

    User GetUserByID(int userId);

    void AddUser(User user);

    public bool GetUserFromSession(ISession session, out User user);

    public bool GetUserByEmail(string email, out User? u);

    User UpdateUser(int id, User newUser);

    void Delete();
    void AddTemporaryUser(string email);
}