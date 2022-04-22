using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DayOfFun.Model
{
    public class Question
    {
        [ForeignKey("QuestionId")]
        public int Id { get; set; }
        
        [Display(Name = "Question text")]
        [Required(ErrorMessage = "Question text is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage ="text must be between 1 and 1000 chars")]
        public String Text { get; set; }

        public Boolean Enabled { get; set; }

        public String? Tags { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }

        public Question()
        {
            this.Text = "DEFAULT";
        }
    }
}
