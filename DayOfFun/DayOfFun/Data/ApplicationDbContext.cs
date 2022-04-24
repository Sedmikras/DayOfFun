using DayOfFun.Model;
using DayOfFun.Models;
using DayOfFun.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace DayOfFun.Data;

public class ApplicationDbContext : DbContext
{
    public static ILoggerFactory _logger = LoggerFactory.Create(builder => builder.AddConsole());
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Quizzes_Users> Quizzes_Users { get; set; }
    public DbSet<Question> Questions { get; set; }
    
    public DbSet<Quizzes_Users>? Question_Users { get; set; }
    public DbSet<Quizzes_Quesitons> Quizzes_Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Quizzes_Users>().HasKey(qu => new {qu.quizId, qu.userId}).HasName("IX_Question_Users_MultipleColumns");
        modelBuilder.Entity<Quizzes_Quesitons>().HasKey(qq => new {qq.QuizID, qq.quesitonId}).HasName("IX_Quizzes_Questions_MultipleColumns");
        modelBuilder.Entity<Answer>()
            .HasKey(c => new {c.QuestionId, c.QuizId, c.UserId}).HasName("IX_Answers_MultipleColumns");

        modelBuilder.Entity<Quizzes_Users>()
            .HasOne(user => user.user)
            .WithMany(qu => qu.Quizzes_Users)
            .HasForeignKey(u => u.userId);

        modelBuilder.Entity<Quizzes_Users>()
            .HasOne(user => user.quiz)
            .WithMany(qu => qu.Quizzes_Users)
            .HasForeignKey(u => u.quizId);

        modelBuilder.Entity<User>().HasData(
            new {Id = 1, Email = "zmikundkras@seznam.cz", Username = "Přéma", Password="1234"},
            new {Id = 2, Email = "falafelvtortile@email.cz", Username = "Baru", Password="12345"}
        );

        modelBuilder.Entity<Quiz>().HasData(
            new {Id = 1, Title = "Co budeme dělat ?", OwnerId = 1, State = State.CREATED}
        );

        modelBuilder.Entity<Quizzes_Users>().HasData(
            new {Id = 1, userId = 1, quizId = 1},
            new {Id = 2, userId = 2, quizId = 1}
        );

        modelBuilder.Entity<Question>().HasData(
            new {Enabled = true, Id = 1, Text = "Pracovat"},
            new {Enabled = true, Id = 2, Text = "Okopávat záhonek"},
            new {Enabled = true, Id = 3, Text = "Nic"},
            new {Enabled = true, Id = 4, Text = "Běhat"},
            new {Enabled = true, Id = 5, Text = "Divadlo"},
            new {Enabled = true, Id = 6, Text = "Šopíkovat oblečení"},
            new {Enabled = true, Id = 7, Text = "Hrát na housle"}
        );

        modelBuilder.Entity<Answer>().HasData(
            new {Id = 1, UserId = 1, QuestionId = 1, QuizId = 1, Result = Result.IF_MUST},
            new {Id = 2, UserId = 1, QuestionId = 2, QuizId = 1, Result = Result.YES},
            new {Id = 3, UserId = 1, QuestionId = 3, QuizId = 1, Result = Result.IF_MUST},
            new {Id = 4, UserId = 1, QuestionId = 4, QuizId = 1, Result = Result.IF_MUST},
            new {Id = 5, UserId = 1, QuestionId = 5, QuizId = 1, Result = Result.IF_MUST},
            new {Id = 6, UserId = 1, QuestionId = 6, QuizId = 1, Result = Result.YES},
            new {Id = 7, UserId = 1, QuestionId = 7, QuizId = 1, Result = Result.IF_MUST},
            new {Id = 8, UserId = 2, QuestionId = 1, QuizId = 1, Result = Result.IF_MUST},
            new {Id = 9, UserId = 2, QuestionId = 2, QuizId = 1, Result = Result.YES},
            new {Id = 10, UserId = 2, QuestionId = 3, QuizId = 1, Result = Result.NO},
            new {Id = 11, UserId = 2, QuestionId = 4, QuizId = 1, Result = Result.YES},
            new {Id = 12, UserId = 2, QuestionId = 5, QuizId = 1, Result = Result.IF_MUST},
            new {Id = 13, UserId = 2, QuestionId = 6, QuizId = 1, Result = Result.IF_MUST},
            new {Id = 14, UserId = 2, QuestionId = 7, QuizId = 1, Result = Result.YES}
        );

        modelBuilder.Entity<Quizzes_Quesitons>().HasData(
            new {Id = 1, QuizID = 1, quesitonId = 1},
            new {Id = 2, QuizID = 1, quesitonId = 2},
            new {Id = 3, QuizID = 1, quesitonId = 3},
            new {Id = 4, QuizID = 1, quesitonId = 4},
            new {Id = 5, QuizID = 1, quesitonId = 5},
            new {Id = 6, QuizID = 1, quesitonId = 6},
            new {Id = 7, QuizID = 1, quesitonId = 7}
        );

        /*
        //USERS
        if (!Users.Any())
        {
            Users.AddRange(new List<User>()
            {
                new User() {Email = "zmikundkras@seznam.cz", Name = "Přéma"},
                new User() {Email = "falafelvtortile@email.cz", Name = "Baru"}
            });
            this.SaveChanges();
        }

        //QUIZZES
        if (!Quizzes.Any())
        {
            Quizzes.AddRange(new List<Quiz>()
            {
                new Quiz()
                {
                    Title = "Co budeme dělat odpoledne ?"
                }
            });
            SaveChanges();
        }*/
        /*
        //CREATE
        if (!Quizzes_Users.Any())
        {
            Quizzes_Users.AddRange(new List<Quizzes_Users>()
            {
                new Quizzes_Users()
                {
                    quizId = 1,
                    userId = 1
                }
            });
            SaveChanges();
        }*/

        /*
        Quiz quiz = 
            
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
        */ /*
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

        /*
        modelBuilder.Entity<Quiz_Questions>().HasKey(
            qq => new {qq.QuestionId, qq.QuizId}
        );

        modelBuilder.Entity<Quiz_Questions>()
            .HasOne(qq => qq.quiz)
            .WithMany(qq => qq.Questions)
            .HasForeignKey(q => q.QuizId);
        
        

        modelBuilder.Entity<User>()
            .OwnsMany(user => user.Quizzes)
            .HasData(
                user1, user2
        );

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
            )
            .Ow(quiz => quiz.Owner)
            .HasData(
                new {Id = 1, Title = "Co budeme dělat?", Owner = user1, State = State.FINISHED}
            );*/

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
}