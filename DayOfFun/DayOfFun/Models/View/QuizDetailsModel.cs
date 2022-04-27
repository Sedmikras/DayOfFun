using DayOfFun.Model;

namespace DayOfFun.Models.View;

public class QuizDetailsModel
{
    public String Title { get; set; }

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

    public WeightedAnswers()
    {
        Answers = new List<Answer>();
    }
}