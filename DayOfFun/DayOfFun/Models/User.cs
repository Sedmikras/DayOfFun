using System.ComponentModel.DataAnnotations.Schema;
using DayOfFun.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DayOfFun.Model
{
    public class User : IdentityUser<int>
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]

        public string? Name { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(255)")]
        public String? Email { get; set; }

        //RELATIONSHIPS
        public virtual List<Quizzes_Users> Quizzes_Users { get; set; } = new List<Quizzes_Users>();

        public User() : base()
        {
        }
    }
}