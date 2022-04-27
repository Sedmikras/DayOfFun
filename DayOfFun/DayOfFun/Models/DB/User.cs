using System.ComponentModel.DataAnnotations;
using DayOfFun.Models.View;

namespace DayOfFun.Model
{
    public class User
    {
        [Key] public int Id { get; set; }

        public string? Username { get; set; }

        public String Email { get; set; }

        public bool IsTemporary { get; set; } = false;

        public string? Password { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }

        public User() : base()
        {
            Quizzes = new HashSet<Quiz>();
        }
    }
}