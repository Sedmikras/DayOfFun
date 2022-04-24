using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DayOfFun.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DayOfFun.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Username is required with maximum of 100 chars")]
        public string? Username { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(255)")]
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Passwords does not match")]
        [DataType(DataType.Password)]
        [NotMapped]
        public string ConfirmPassword { get; set; }
        
        

        //RELATIONSHIPS
        public virtual List<Quizzes_Users> Quizzes_Users { get; set; } = new List<Quizzes_Users>();

        public User() : base()
        {
        }
    }
}