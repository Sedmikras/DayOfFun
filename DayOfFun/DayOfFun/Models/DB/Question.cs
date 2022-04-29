using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DayOfFun.Models.DB
{
    public class Question
    {
        [ForeignKey("QuestionId")] public int Id { get; set; }

        [Display(Name = "Question text")]
        [Required(ErrorMessage = "Question text is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "text must be between 1 and 1000 chars")]
        public string Text { get; set; }

        public bool Enabled { get; set; }

        public string? Tags { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }

        public Question()
        {
            Quizzes = new HashSet<Quiz>();
        }

        public override bool Equals(object? obj)
        {
            return ((Question) obj).Text == Text;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }
    }
}