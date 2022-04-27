﻿using DayOfFun.Model;
using DayOfFun.Models;
using DayOfFun.Models.View;

namespace DayOfFun.Data.Services.Contract;

public interface IUserService
{
    bool RegisterUser(UserViewModel uvm, out User user);

    bool getQuizzIdsForUser(ISession session, out List<Quizzes_Users> quizzesUsers);

    User getUserByID(int userId);

    void addUser();

    public bool getUserFromSession(ISession session, out User user);

    public bool GetUserByEmail(string email, out User u);

    User updateUser(int id, User newUser);

    void Delete();
    void AddTemporaryUser(string email);
}