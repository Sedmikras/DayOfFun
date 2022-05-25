using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DayOfFun.Models.DB;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;

namespace DayOfFun.Models.View;
/// <summary>
/// In this file there are variants of users used in views
/// </summary>
public class UserViewModel
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    [Required(ErrorMessage = "Username is required with maximum of 100 chars")]
    public string Username { get; set; }

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

/// <summary>
/// Share user view - needed only quiz ID and email
/// </summary>
public class ShareUserViewModel
{
    [PersonalData]
    [Column(TypeName = "nvarchar(255)")]
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$")]
    public string Email { get; set; }
    
    public int QuizId { get; set; }
}

/// <summary>
/// User details model - info about user and his commitment to the quiz (if he started the quiz and so on)
/// </summary>
public class UserDetailsModel
{
    public string Username { get; set; }
    public string Email { get; set; }

    public int QuizId { get; set; }
    public bool IsFinished { get; set; } = false;

    public bool IsResponded { get; set; } = false;

    public int NumberOfQuestions { get; set; } = 0;

    public int NumberOfAnswers { get; set; } = 0;

}