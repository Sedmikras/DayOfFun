using DayOfFun.Model;

namespace DayOfFun.BeginCollectionItemCore.Models;

/// <summary>
/// Simplified model for Index check of quizzes
/// </summary>
public class QuizViewModel
{
    public int Id { get; set; }
    public String Title { get; set; }
    public State State { get; set; }
}