using DayOfFun.Model;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.context;

public class QuizContext : DbContext
{
    public static ILoggerFactory _logger = LoggerFactory.Create(builder => builder.AddConsole());
    public DbSet<Quiz> quizes { get; set; }
    public DbSet<Answer> answersSet { get; set; }
    public DbSet<User> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        User user1 = new User() {Id = 1, Email = "zmikundkras@seznam.cz", Name = "Přéma"};
        User user2 = new User() {Id = 2, Email = "falafelvtortile@email.cz", Name = "Baru"};

        modelBuilder.Entity<User>().HasData(
            user1, user2
        );

        Quiz quiz = new Quiz()
        {
            Id = 1, Owner = user1, Title = "Co budeme dělat odpoledne ?", Users = null, WaitingUsers = null
        };

        Question q1 = new Question() {Enabled = true, Id = 1, Text = "Pracovat"};
        Question q2 = new Question() {Enabled = true, Id = 2, Text = "Okopávat záhonek"};
        Question q3 = new Question() {Enabled = true, Id = 3, Text = "Nic"};
        Question q4 = new Question() {Enabled = true, Id = 4, Text = "Běhat"};
        Question q5 = new Question() {Enabled = true, Id = 5, Text = "Divadlo"};
        Question q6 = new Question() {Enabled = true, Id = 6, Text = "Šopíkovat oblečení"};
        Question q7 = new Question() {Enabled = true, Id = 7, Text = "Hrát na housle"};
        HashSet<Question> questions = new HashSet<Question>() {q1, q2, q3, q4, q5, q6, q7};
        quiz.AddQuestions(questions);

        Answer a1 = new Answer()
            {Id = 1, UserId = user1.Id, QuestionId = q1.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a2 = new Answer() {Id = 2, UserId = user1.Id, QuestionId = q2.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a3 = new Answer()
            {Id = 3, UserId = user1.Id, QuestionId = q3.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a4 = new Answer()
            {Id = 4, UserId = user1.Id, QuestionId = q4.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a5 = new Answer()
            {Id = 5, UserId = user1.Id, QuestionId = q5.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a6 = new Answer() {Id = 6, UserId = user1.Id, QuestionId = q6.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a7 = new Answer()
            {Id = 7, UserId = user1.Id, QuestionId = q7.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        HashSet<Answer> answers = new HashSet<Answer>() {a1, a2, a3, a4, a5, a6, a7};
        quiz.Update(answers, user1);

        Answer a8 = new Answer()
            {Id = 8, UserId = user2.Id, QuestionId = q1.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a9 = new Answer() {Id = 9, UserId = user2.Id, QuestionId = q2.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a10 = new Answer()
            {Id = 10, UserId = user2.Id, QuestionId = q3.Id, QuizId = quiz.Id, Result = Result.NO};
        Answer a11 = new Answer()
            {Id = 11, UserId = user2.Id, QuestionId = q4.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a12 = new Answer()
            {Id = 12, UserId = user2.Id, QuestionId = q5.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a13 = new Answer()
            {Id = 13, UserId = user2.Id, QuestionId = q6.Id, QuizId = quiz.Id, Result = Result.IF_MUST};
        Answer a14 = new Answer()
            {Id = 14, UserId = user2.Id, QuestionId = q7.Id, QuizId = quiz.Id, Result = Result.YES};
        answers = new HashSet<Answer>() {a8, a9, a10, a11, a12, a13, a14};
        quiz.Update(answers, user2);

        Question q8 = new Question() {Enabled = true, Id = 8, Text = "Vařit"};
        Question q9 = new Question() {Enabled = true, Id = 9, Text = "Péct"};
        Question q10 = new Question() {Enabled = true, Id = 10, Text = "Výletit"};
        questions = new HashSet<Question>() {q8, q9, q10};
        quiz.AddQuestions(questions, new HashSet<User>() {user2});

        Answer a15 = new Answer()
            {Id = 15, UserId = user2.Id, QuestionId = q8.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a16 = new Answer()
            {Id = 16, UserId = user2.Id, QuestionId = q9.Id, QuizId = quiz.Id, Result = Result.YES};
        Answer a17 = new Answer()
            {Id = 17, UserId = user2.Id, QuestionId = q10.Id, QuizId = quiz.Id, Result = Result.YES};
        answers = new HashSet<Answer>() {a15, a16, a17};
        quiz.Update(answers, user2);

        Answer a18 = new Answer()
            {Id = 18, UserId = user1.Id, QuestionId = q8.Id, QuizId = quiz.Id, Result = Result.NO};
        Answer a19 = new Answer()
            {Id = 19, UserId = user1.Id, QuestionId = q9.Id, QuizId = quiz.Id, Result = Result.NO};
        Answer a20 = new Answer()
            {Id = 20, UserId = user1.Id, QuestionId = q10.Id, QuizId = quiz.Id, Result = Result.YES};
        answers = new HashSet<Answer>() {a18, a19, a20};
        quiz.Update(answers, user1);

        modelBuilder.Entity<Answer>().HasData(
            a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17, a18, a19, a20
        );

        modelBuilder.Entity<Question>().HasData(
            q1, q2, q3, q4, q5, q6, q7, q8, q9, q10
        );

        modelBuilder.Entity<Quiz>()
            .HasMany(quizEntity => quizEntity.Questions)
            .WithMany(questionEntity => questionEntity.Quizzes)
            .UsingEntity<QuizQuestions>(j => j.HasData(
                    new {QuizId = 1, QuestionId = 1},
                    new {QuizId = 1, QuestionId = 2},
                    new {QuizId = 1, QuestionId = 3},
                    new {QuizId = 1, QuestionId = 4},
                    new {QuizId = 1, QuestionId = 5},
                    new {QuizId = 1, QuestionId = 6},
                    new {QuizId = 1, QuestionId = 7},
                    new {QuizId = 1, QuestionId = 8},
                    new {QuizId = 1, QuestionId = 9},
                    new {QuizId = 1, QuestionId = 10}
                )
            ).HasData(
                new {Id = 1, Title = "Co budeme dělat?", Owner = user1, State = State.FINISHED}
            );

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=DayOfFun;Trusted_Connection=True");
        optionsBuilder.UseLoggerFactory(_logger);
        optionsBuilder.UseLazyLoadingProxies();
        //TODO
        optionsBuilder.EnableSensitiveDataLogging();
    }

    void SaveQuiz(Quiz quiz)
    {
    }

    List<Quiz> GetQuizesByUser(User user)
    {
        return GetQuizesByUserId(user.Id);
    }

    List<Quiz> GetQuizesByUserId(int userId)
    {
        return null;
    }

    Quiz GetQuizById(int id)
    {
        return null;
    }
}