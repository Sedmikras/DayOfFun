using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DayOfFun.Models.DB
{
    public class User
    {
        [Key] public int Id { get; set; }

        public string? Username { get; set; }

        [PersonalData]
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$")]
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