using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DayOfFun.Models.DB;

namespace DayOfFun.Models.View;

[NotMapped]
public class QuizAnswerModel
{
    public int UserId { get; set; }
    public int QuizId { get; set; }

    public string Title { get; set; }

    private bool AlreadyFilled { get; set; } = false;

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

public class QuizCreateViewModel : QuizViewModel
{
    public virtual List<Question> Questions { get; set; } = new List<Question>();
}

public class QuizDetailsModel
{
    public string Title { get; set; }

    public Dictionary<string, WeightedAnswers> Answers { get; set; }

    public QuizDetailsModel(Quiz quiz, List<Answer> answers)
    {
        Answers = new Dictionary<string, WeightedAnswers>();
        Title = quiz.Title;
        foreach (var question in quiz.Questions)
        {
            Answers.Add(question.Text, new WeightedAnswers(answers.FindAll(a => a.QuestionId == question.Id)));
        }

        Answers = new Dictionary<string, WeightedAnswers>(Answers.OrderByDescending(a => a.Value.Score));
    }
}

public class WeightedAnswers
{
    public double Score { get; } = 0;

    public int Respondents { get; } = 0;

    public bool IsDoable { get; set; } = true;

    private List<Answer> Answers { get; }

    public WeightedAnswers(List<Answer> pAnswers)
    {
        Answers = new List<Answer>();
        foreach (var answer in pAnswers)
        {
            if (answer.Result == Result.YES)
            {
                Score += 1;
                Respondents++;
            }
            else if (answer.Result == Result.IF_MUST)
            {
                Score += 0.5;
                Respondents++;
            }

            if (answer.Result == Result.NO)
            {
                Score += 0;
                Respondents++;
                IsDoable = true;
            }
        }
    }
}

/// <summary>
/// Simplified model for Index check of quizzes
/// </summary>
public class QuizViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public State State { get; set; }
}