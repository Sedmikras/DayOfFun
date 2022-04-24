using DayOfFun.Model;
using DayOfFun.Models.Domain;

namespace DayOfFun.Data.Services.Contract;

public interface IQuizService
{
    public void AddQuiz(Quiz quiz, ISession session);

    public List<Quiz> getQuizzesForUser(User user);

    void Delete(ISession httpContextSession, int id);

    public Quiz_Answer_Model getQuestionsFor(ISession httpContextSession, int id);

    public void ValidateModel(Quiz_Answer_Model model);
}