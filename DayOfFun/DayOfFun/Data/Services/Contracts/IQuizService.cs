using DayOfFun.Model;
using DayOfFun.Models.Domain;

namespace DayOfFun.Data.Services.Contract;

/// <summary>
/// 
/// </summary>
public interface IQuizService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="quiz"></param>
    /// <param name="session"></param>
    public void AddQuiz(Quiz quiz, ISession session);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public List<Quiz> getQuizzesForUser(User user);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextSession"></param>
    /// <param name="id"></param>
    void Delete(ISession httpContextSession, int id);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextSession"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Quiz_Answer_Model getQuestionsFor(ISession httpContextSession, int id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    public void ValidateModel(Quiz_Answer_Model model);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="quizId"></param>
    /// <returns></returns>
    public Quiz getQuizById(int quizId);
}