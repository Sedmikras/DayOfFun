using DayOfFun.BeginCollectionItemCore.Models;
using DayOfFun.Model;

namespace DayOfFun.Models.View;

public class QuizCreateViewModel : QuizViewModel
{
    public virtual List<Question> questions { get; set; } = new List<Question>();
}