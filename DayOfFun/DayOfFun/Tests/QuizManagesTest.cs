using DayOfFun.Model;
using NUnit.Framework;
namespace WebApplication1.tests;


public class QuizManagesTest
{
    [Test]
    public void HappyDayScenarioSimpleOnlyOwner()
    {
        User user1 = new User() {Id = 1, Email = "zmikundkras@seznam.cz", Name = "Přéma"};
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
        Answer a1 = new Answer() {Id = 1, UserId = user1.Id, QuestionId = q1.Id, QuizId = quiz.Id, Result = Result.NO};
        HashSet<Answer> answers = new HashSet<Answer>() {a1};
        quiz.Update(answers, user1);
        Assert.IsTrue(quiz.State == State.FINISHED);
    }
    
    [Test]
    public void HappyDayScenarioComplexTwoUsers()
    {
        User user1 = new User() {Id = 1, Email = "zmikundkras@seznam.cz", Name = "Přéma"};
        User user2 = new User() {Id = 2, Email = "falafelvtortile@email.cz", Name = "Baru"};
        Quiz quiz = new Quiz()
        {
            Id = 1, Owner = user1, Title = "Co budeme dělat odpoledne ?", Users = null, WaitingUsers = null
        };
        Assert.IsTrue(quiz.State == State.CREATED);
        Question q1 = new Question() {Enabled = true, Id = 1, Text = "Pracovat"};
        Question q2 = new Question() {Enabled = true, Id = 2, Text = "Okopávat záhonek"};
        Question q3 = new Question() {Enabled = true, Id = 3, Text = "Nic"};
        Question q4 = new Question() {Enabled = true, Id = 4, Text = "Běhat"};
        Question q5 = new Question() {Enabled = true, Id = 5, Text = "Divadlo"};
        Question q6 = new Question() {Enabled = true, Id = 6, Text = "Šopíkovat oblečení"};
        Question q7 = new Question() {Enabled = true, Id = 7, Text = "Hrát na housle"};
        List<Question> questions = new List<Question>() {q1, q2, q3, q4, q5, q6, q7};
        quiz.AddQuestions(questions, new HashSet<User>(){user2});
        Assert.IsTrue(quiz.State == State.PREPARED);
        
        Answer a1 = new Answer() {Id = 1, UserId = user1.Id, QuestionId = q1.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a2 = new Answer() {Id = 2, UserId = user1.Id, QuestionId = q2.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a3 = new Answer() {Id = 3, UserId = user1.Id, QuestionId = q3.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a4 = new Answer() {Id = 4, UserId = user1.Id, QuestionId = q4.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a5 = new Answer() {Id = 5, UserId = user1.Id, QuestionId = q5.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a6 = new Answer() {Id = 6, UserId = user1.Id, QuestionId = q6.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a7 = new Answer() {Id = 7, UserId = user1.Id, QuestionId = q7.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        HashSet<Answer> answers = new HashSet<Answer>() {a1, a2, a3, a4, a5, a6, a7};
        quiz.Update(answers, user1);
        Assert.IsTrue(quiz.State == State.WAITING);
        
        Answer a8 = new Answer() {Id = 8, UserId = user2.Id, QuestionId = q1.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a9 = new Answer() {Id = 9, UserId = user2.Id, QuestionId = q2.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a10 = new Answer() {Id = 10, UserId = user2.Id, QuestionId = q3.Id, QuizId = quiz.Id, Result = Result.NO};
        Answer a11 = new Answer() {Id = 11, UserId = user2.Id, QuestionId = q4.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a12 = new Answer() {Id = 12, UserId = user2.Id, QuestionId = q5.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a13 = new Answer() {Id = 13, UserId = user2.Id, QuestionId = q6.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a14 = new Answer() {Id = 14, UserId = user2.Id, QuestionId = q7.Id, QuizId = quiz.Id, Result = Result.YES};
        answers = new HashSet<Answer>() {a8, a9, a10, a11, a12, a13, a14};
        quiz.Update(answers, user2);
        Assert.IsTrue(quiz.State == State.FINISHED);
        
        Question q8 = new Question() {Enabled = true, Id = 8, Text = "Vařit"};
        Question q9 = new Question() {Enabled = true, Id = 9, Text = "Péct"};
        Question q10 = new Question() {Enabled = true, Id = 10, Text = "Výletit"};
        questions = new List<Question>() {q8, q9, q10};
        quiz.AddQuestions(questions);
        Assert.IsTrue(quiz.State == State.WAITING);
        
        Answer a15 = new Answer() {Id = 15, UserId = user2.Id, QuestionId = q8.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a16 = new Answer() {Id = 16, UserId = user2.Id, QuestionId = q9.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a17 = new Answer() {Id = 17, UserId = user2.Id, QuestionId = q10.Id, QuizId = quiz.Id, Result = Result.YES};
        answers = new HashSet<Answer>() {a15, a16, a17};
        quiz.Update(answers, user2);
        Assert.IsTrue(quiz.State == State.WAITING);

        Assert.IsTrue(quiz.WaitingUsers.Count == 1 && quiz.WaitingUsers.Contains(user1));
        
        Answer a18 = new Answer() {Id = 18, UserId = user1.Id, QuestionId = q8.Id, QuizId = quiz.Id, Result = Result.NO};
        Answer a19 = new Answer() {Id = 19, UserId = user1.Id, QuestionId = q9.Id, QuizId = quiz.Id, Result = Result.NO};
        Answer a20 = new Answer() {Id = 20, UserId = user1.Id, QuestionId = q10.Id, QuizId = quiz.Id, Result = Result.YES};
        answers = new HashSet<Answer>() {a18, a19, a20};
        quiz.Update(answers, user1);
        Assert.IsTrue(quiz.State == State.FINISHED);
    }
}