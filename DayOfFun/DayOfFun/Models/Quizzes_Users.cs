using DayOfFun.Model;

namespace DayOfFun.Models;

public class Quizzes_Users
{
    public int quizId { get; set; }
    public int userId { get; set; }
    public User user { get; set; }
    public Quiz quiz { get; set; }
}