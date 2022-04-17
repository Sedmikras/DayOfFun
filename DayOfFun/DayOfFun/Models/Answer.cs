using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DayOfFun.Model
{
    public class Answer
    {
        public Result Result
        {
            get;
            set;
        }
        public int UserId
        {
            get;
            init;
        }

        [ForeignKey("QuizId")]
        public int QuizId
        {
            get;
            set;
        }

        [ForeignKey("QuestionId")]
        public int QuestionId
        {
            get; init; 
            
        }
        [Key]
        public int Id {
            get; 
            set; 
        }
    }

    public enum Result : byte
    {
        NO = 0,
        IF_MUST = 1,
        YES =2
    }
}
