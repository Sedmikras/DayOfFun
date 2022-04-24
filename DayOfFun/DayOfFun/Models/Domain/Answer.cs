using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DayOfFun.Model
{
    public class Answer
    {
        public Answer()
        {
            Result = Result.YES;
        }
        
        [Display(Name = "Question answer")]
        [Required(ErrorMessage = "Answer is required")]
        public Result Result
        {
            get;
            set;
        }
        
        [Display(Name = "UserId")]
        [Required(ErrorMessage = "UserId is mandatory")]
        public int UserId
        {
            get;
            init;
        }

        [Display(Name = "QuizId")]
        [Required(ErrorMessage = "QuizId is mandatory")]
        [ForeignKey("QuizId")]
        public int QuizId
        {
            get;
            set;
        }

        [Display(Name = "QuestionId")]
        [Required(ErrorMessage = "QuestionId is mandatory")]
        [ForeignKey("QuestionId")]
        public int QuestionId
        {
            get; init; 
            
        }
    }

    public enum Result : byte
    {
        [Display(Name = "NO")]
        NO = 0,
        [Display(Name = "IF XY WANTS")]
        IF_MUST = 1,
        [Display(Name = "YES")]
        YES =2
    }
}
