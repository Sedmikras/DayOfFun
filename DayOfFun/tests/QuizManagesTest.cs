using DayOfFun.Model;
using NUnit.Framework;
namespace WebApplication1.tests;


public class QuizManagesTest
{
    [Test]
    public void HappyDayScenario()
    {
        User user1 = new User() {Id = 1, Email = "zmikundkras@seznam.cz", Name = "Přéma"};
        User user2 = new User() {Id = 2, Email = "flafelvtortile@email.cz", Name = "Baru"};
        Quiz quiz = new Quiz()
        {
            Id = 1, Owner = user1, Title = "Co budeme dělat odpoledne", Users = null,
            WaitingUsers = null
        };
        Assert.IsTrue(quiz.State == State.CREATED);
        Question q1 = new Question() {Enabled = true, Id = 1, Text = "Budeme běhat"};
        List<Question> questions = new List<Question>() {q1};
        quiz.Questions = questions;
        Assert.IsTrue(quiz.State == State.PREPARED);
        Answer a1 = new Answer() {Id = 1, UserId = user1.Id, QuestionID = q1.Id, QuizID = quiz.Id, Result = 1};
        List<Answer> answers = new List<Answer>() {a1};
        quiz.Update(answers);
        Assert.IsTrue(quiz.State == State.FINISHED);
    }
}