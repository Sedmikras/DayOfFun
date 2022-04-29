using System.ComponentModel.DataAnnotations;

namespace DayOfFun.Models.DB
{
    public class User
    {
        [Key] public int Id { get; set; }

        public string? Username { get; set; }

        public string Email { get; set; }

        public bool IsTemporary { get; set; } = false;

        public string? Password { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }

        public User() : base()
        {
            Quizzes = new HashSet<Quiz>();
        }
    }
}