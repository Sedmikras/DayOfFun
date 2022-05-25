using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.Data.Services.Contracts;

public interface IUserService
{
    /// <summary>
    /// registers user - save user to the database
    /// </summary>
    /// <param name="uvm">info about users - email (unique) / password / username</param>
    /// <returns>true if success / false if error</returns>
    bool RegisterUser(UserViewModel uvm);

    /// <summary>
    /// Return user from DB given information in session (logged user)
    /// </summary>
    /// <param name="session">Session with info about users</param>
    /// <param name="user">User instance from DB given the info from session</param>
    /// <returns>true if success / false if error</returns>
    public bool GetUserFromSession(ISession session, out User user);

    /// <summary>
    /// Return user from DB given email (temporary login from public access) 
    /// </summary>
    /// <param name="email">email of the public (non-registered, non-logged) user</param>
    /// <param name="u">User instance from DB given the info from session</param>
    /// <returns>true if success / false if error</returns>
    public bool GetUserByEmail(string email, out User? u);

    //void AddTemporaryUser(string email);
    
    
    // --------------------------------- NOT ENOUGH TIME TO FINISH THIS
    /*
    User UpdateUser(int id, User newUser);

    User GetUserByID(int userId);
    
    void Delete();

    void AddUser(User user);*/
}