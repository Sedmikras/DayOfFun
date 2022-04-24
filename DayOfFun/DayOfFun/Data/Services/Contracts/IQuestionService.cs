using DayOfFun.Model;
using DayOfFun.Models.Domain;

namespace DayOfFun.Data.Services.Contract;

public interface IQuestionService
{
    public void AddQuestionForQuiz(Question question, Quiz quiz);
    List<Question> getQuestionsForQuizAndUser(Quiz quiz, User user);
}