using DayOfFun.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace DayOfFun.Data;

public class ApplicationDbContext : DbContext
{
    public static ILoggerFactory _logger = LoggerFactory.Create(builder => builder.AddConsole());
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
        optionsBuilder.UseLoggerFactory(_logger);
        optionsBuilder.UseLazyLoadingProxies();
        //TODO
        optionsBuilder.EnableSensitiveDataLogging();
    }
}