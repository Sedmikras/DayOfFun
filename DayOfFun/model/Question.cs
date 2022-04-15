using System.ComponentModel.DataAnnotations.Schema;

namespace DayOfFun.Model
{
    public class Question
    {
        [ForeignKey("QuestionId")]
        public int Id { get; set; }

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
