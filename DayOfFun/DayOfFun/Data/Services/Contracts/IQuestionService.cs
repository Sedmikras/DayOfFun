using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.Data.Services.Contract;

public interface IQuestionService
{
    public bool AddQuestionForQuiz(Question question, Quiz quiz);

    public bool AddQuestionsForQuiz(Quiz quiz);

    List<Question> getQuestionsForQuizAndUser(Quiz quiz, User user);

    public void removeDuplicities(Quiz quiz, out List<Question> questions);

    public List<Question> getQuestionsFromModel(QuizCreateViewModel quizModel);
}