using DayOfFun.Models.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DayOfFun.Data;

public class ApplicationDbContext : DbContext
{
    private static readonly ILoggerFactory Logger = LoggerFactory.Create(builder => builder.AddConsole());
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>()
            .HasKey(c => new {c.QuestionId, c.QuizId, c.UserId}).HasName("IX_Answers_MultipleColumns");
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=DayOfFun;Trusted_Connection=True");
        optionsBuilder.UseLoggerFactory(Logger);
        optionsBuilder.UseLazyLoadingProxies();
        //TODO
        optionsBuilder.EnableSensitiveDataLogging();
    }
}