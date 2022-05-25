using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DayOfFun.Models.DB
{
    public class Answer
    {
        public Answer()
        {
            Result = Result.YES;
        }

        [Display(Name = "Question answer")]
        [Required(ErrorMessage = "Answer is required")]
        public Result Result { get; set; } 

        [Display(Name = "UserId")]
        [Required(ErrorMessage = "UserId is mandatory")]
        public int UserId { get; init; }
        
        [ForeignKey("UserId")] public virtual User User { get; set; }

        [Display(Name = "QuizId")]
        [Required(ErrorMessage = "QuizId is mandatory")]
        public int QuizId { get; set; }

        [ForeignKey("QuizId")] public virtual Quiz Quiz { get; set; }

        [Display(Name = "QuestionId")]
        [Required(ErrorMessage = "QuestionId is mandatory")]
        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")] public virtual Question Question { get; set; }
    }

    public enum Result : byte
    {
        [Display(Name = "NO")] NO = 0,
        [Display(Name = "IF OTHER WANTS")] IF_MUST = 1,
        [Display(Name = "YES")] YES = 2
    }
}