using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DayOfFun.BeginCollectionItemCore.Models;
using DayOfFun.Models.Domain;
using DayOfFun.Models.View;
using NuGet.Packaging;

namespace DayOfFun.Model
{
    public enum State
    {
        CREATED,
        PREPARED,
        DONE,
        WAITING,
        FINISHED,
        ARCHIVED,
        INVALID
    }

    public class Quiz
    {
        [Key] public int Id { get; set; }

        [Display(Name = "Quiz title")]
        [Required(ErrorMessage = "Quiz title is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 chars")]

        public String Title { get; set; }

        public State State { get; set; }

        public String? Tags;

        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")] [Column] public User Owner;

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public Quiz()
        {
            this.Users = new HashSet<User>();
            this.Questions = new HashSet<Question>();
        }

        public Quiz(User u, QuizCreateViewModel qvm)
        {
            this.Users = new HashSet<User>();
            this.Questions = new HashSet<Question>();
            if (qvm.questions.Count == 0)
                this.State = State.CREATED;
            this.State = State.PREPARED;
            this.Questions.AddRange(qvm.questions);
            this.Owner = u;
            this.OwnerId = u.Id;
            this.Title = qvm.Title;
            this.Users.Add(u);
        }

        public QuizViewModel ToViewModel()
        {
            return new QuizViewModel()
            {
                Id = Id,
                State = State,
                Title = Title
            };
        }

        public QuizAnswerModel ToAnswerModel(User u)
        {
            var actualQuestions = this.Questions.Select(question => new AnswerView()
                {
                    QuizId = this.Id,
                    QuestionId = question.Id,
                    UserId = u.Id,
                })
                .ToList();
            return new QuizAnswerModel()
            {
                Questions = this.Questions.ToList(),
                Title = this.Title,
                QuizId = this.Id,
                UserId = u.Id,
                QuestionAnswers = actualQuestions
            };
        }

        public List<Answer> AddAnswers(List<AnswerView> view, User u)
        {
            List<Answer> answers = new List<Answer>();

            foreach (var answerView in view)
            {
                Question q = Questions.FirstOrDefault(q => q.Id == answerView.QuestionId);
                if (q == null)
                {
                    throw new Exception();
                }

                answers.Add(new Answer()
                {
                    Question = q,
                    Quiz = this,
                    User = u,
                    QuizId = this.Id,
                    Result = answerView.Result,
                    UserId = answerView.UserId,
                    QuestionId = answerView.QuestionId
                });
            }

            return answers;
        }
    }
}