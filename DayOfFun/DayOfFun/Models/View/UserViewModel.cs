using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DayOfFun.Models.DB;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;

namespace DayOfFun.Models.View;

public class UserViewModel
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    [Required(ErrorMessage = "Username is required with maximum of 100 chars")]
    public string Username{ get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(255)")]
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Passwords does not match")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    public User ToApplicationUser()
    {
        return new User()
        {
            Username = this.Username,
            Password = this.Password,
            Email = this.Email
        };
    }
}

public class ShareUserViewModel
{
    [PersonalData]
    [Column(TypeName = "nvarchar(255)")]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    public int QuizId { get; set; }
}

public class UserDetailsModel
{
    public string Username { get; set; }
    public string Email { get; set; }

    public int QuizId { get; set; }
    public bool IsFinished { get; set; } = false;

    public bool IsResponded { get; set; } = false;

}