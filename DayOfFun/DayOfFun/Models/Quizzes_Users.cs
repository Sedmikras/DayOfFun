using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DayOfFun.Model;

namespace DayOfFun.Models;

public class Quizzes_Users
{
    [Key] public int Id { get; set; }
    public virtual int quizId { get; set; }
    public virtual Quiz quiz { get; set; }
    
    [ForeignKey("UserID")]
    public virtual int userId { get; set; }
    public virtual User user { get; set; }
}