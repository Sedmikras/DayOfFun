using DayOfFun.Models.DB;
using DayOfFun.Models.View;

namespace DayOfFun.Data.Services.Contracts;

public interface IQuestionService
{
    /// <summary>
    /// Add question to the quiz
    /// </summary>
    /// <param name="question">question</param>
    /// <param name="quiz">quiz</param>
    /// <returns>true if success / false if not successful</returns>
    public bool AddQuestionForQuiz(Question question, Quiz quiz);



    /// <summary>
    /// returns list of questions from the DB where questions text is given in parameter (model)
    /// </summary>
    /// <param name="quizModel">list of questions from users input</param>
    /// <returns>List of questions that are in DB</returns>
    public List<Question> GetQuestionsFromModel(QuizCreateViewModel quizModel);
    
    // -------------------------------- MISSING WASNT TIME TO IMPLEMENT
    /*public bool AddQuestionsForQuiz(Quiz quiz);

    List<Question> getQuestionsForQuizAndUser(Quiz quiz, User user);

    public void removeDuplicities(Quiz quiz, out List<Question> questions);*/
}