using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DayOfFun.Model;

namespace DayOfFun.Models.Domain;

[NotMapped]
public class Quiz_Answer_Model
{
    public int UserId { get; set; }
    public int QuizId { get; set; }

    public string Title { get; set; }

    public virtual List<Question> Questions { get; set; } = new List<Question>();
    public virtual List<Answer> QuestionAnswers { get; set; } = new List<Answer>();

    public Quiz_Answer_Model()
    {
    }

    public Quiz_Answer_Model(List<Question> questions, int userId, int quizId, string title)
    {
        Title = title;
        UserId = userId;
        QuizId = quizId;
        Questions = questions;
        foreach (var q in questions)
        {
            QuestionAnswers.Add(new Answer());
        }
    }
}