using DayOfFun.Models.DB;

namespace DayOfFun.Data.Services.Contract;

public interface IQuestionService
{
    public void AddQuestionForQuiz(Question question, Quiz quiz);

    public bool AddQuestionsForQuiz(Quiz quiz);

    List<Question> getQuestionsForQuizAndUser(Quiz quiz, User user);

    public void removeDuplicities(Quiz quiz, out List<Question> questions);

}