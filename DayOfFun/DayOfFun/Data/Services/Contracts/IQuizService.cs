using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.Data.Services.Contract;

/// <summary>
/// 
/// </summary>
public interface IQuizService
{
    /// <summary>
    /// Adds quiz to the DB with all dependencies
    /// </summary>
    /// <param name="quiz">quiz that will be inserted</param>
    /// <param name="user">user inserting quiz</param>
    /// <returns>true if success</returns>
    public bool Add(Quiz quiz, User user);

    /// <summary>
    /// Updates information about quiz in the database
    /// </summary>
    /// <param name="quiz">quiz that will be updated</param>
    /// <returns>true if success</returns>
    public bool Update(Quiz quiz);

    /// <summary>
    /// Removes quiz from the database with check
    /// </summary>
    /// <param name="quizId">id of the quiz</param>
    public bool Delete(int quizId, User u);

    /// <summary>
    /// Removes quiz from the database with check
    /// </summary>
    /// <param name="quizId">instance of the quiz</param>
    /// <returns>true if success</returns>
    public bool Delete(Quiz quiz);

    /// <summary>
    /// Returns quiz from the database and sets its info as output
    /// </summary>
    /// <param name="quizId"> ID of the quiz that will be read </param>
    /// <param name="quiz"> output object with properties from database</param>
    /// <returns> true if success </returns>
    public bool Read(int quizId, out Quiz quiz);
    
    public List<QuizViewModel> GetQuizzesModel(ISession session);

    /// <summary>
    /// Return list of quizzes from the database and for each quiz sets info
    /// list is returned as output 
    /// </summary>
    /// <param name="userId">user Id</param>
    /// <param name="quizzes">output List of quizzes for user</param>
    /// <returns>true if success</returns>
    public bool ReadAllForUser(int userId, out List<Quiz> quizzes);
    
    
    public void AddQuiz(Quiz quiz, ISession session);
    
    public List<Quiz> getQuizzesForUser(User user);

    void Delete(ISession httpContextSession, int id);
    
    public QuizAnswerModel getQuestionsFor(ISession httpContextSession, int id);
    
    public void ValidateModel(Quiz model);
    
    public Quiz getQuizById(int quizId);
    bool ToShareUserViewModel(int quizId, out List<UserDetailsModel> data);
}