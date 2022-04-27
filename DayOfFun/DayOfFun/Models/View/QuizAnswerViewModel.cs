using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DayOfFun.Model;

namespace DayOfFun.Models.Domain;

[NotMapped]
public class QuizAnswerModel
{
    public int UserId { get; set; }
    public int QuizId { get; set; }

    public string Title { get; set; }

    public virtual List<Question> Questions { get; set; } = new List<Question>();
    public virtual List<AnswerView> QuestionAnswers { get; set; } = new List<AnswerView>();

    public QuizAnswerModel()
    {
    }

    public QuizAnswerModel(List<Question> questions, int userId, int quizId, string title)
    {
        Title = title;
        UserId = userId;
        QuizId = quizId;
        Questions = questions;
        foreach (var q in questions)
        {
            QuestionAnswers.Add(new AnswerView());
        }
    }
}

public class AnswerView
{
    [Display(Name = "Question answer")]
    [Required(ErrorMessage = "Answer is required")]
    public Result Result { get; set; } 

    [Display(Name = "UserId")]
    [Required(ErrorMessage = "UserId is mandatory")]
    public int UserId { get; set; }
        

    [Display(Name = "QuizId")]
    [Required(ErrorMessage = "QuizId is mandatory")]
    public int QuizId { get; set; }
    
    [Display(Name = "QuestionId")]
    [Required(ErrorMessage = "QuestionId is mandatory")]
    public int QuestionId { get; set; }
}