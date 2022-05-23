using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DayOfFun.Models.View;
using NuGet.Packaging;

namespace DayOfFun.Models.DB
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

        public string Title { get; set; }

        public State State { get; set; }

        public string? Tags;

        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")] [Column] public User Owner;

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public Quiz()
        {
            Questions = new HashSet<Question>();
            Users = new HashSet<User>();
        }

        public Quiz(User u, QuizCreateViewModel qvm, List<Question> questions)
        {
            Users = new HashSet<User>();
            Questions = new HashSet<Question>();
            if (qvm.Questions.Count == 0)
                State = State.CREATED;
            State = State.PREPARED;
            Questions.AddRange(questions);
            foreach (var question in questions)
            {
                var existingQuestion = qvm.Questions.Find(q => q.Text == question.Text);
                if (existingQuestion != null)
                    qvm.Questions.Remove(existingQuestion);
            }
            Questions.AddRange(qvm.Questions);
            Owner = u;
            OwnerId = u.Id;
            Title = qvm.Title;
            Users.Add(u);
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

        public QuizAnswerModel ToAnswerModel(User u, List<Answer> answers)
        {
            var actualQuestions = this.Questions.Select(question => new AnswerView()
                {
                    QuizId = this.Id,
                    QuestionId = question.Id,
                    UserId = u.Id,
                    Result = answers.Exists(a => a.QuestionId == question.Id)
                        ? answers.First(a => a.QuestionId == question.Id).Result
                        : Result.NO
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

        public IEnumerable<User> ToShareUserViewModel()
        {
            return Users;
        }


        public IEnumerable<Answer> AddAnswers(List<AnswerView> view, User u)
        {
            var answers = new List<Answer>();

            foreach (var answerView in view)
            {
                var q = Questions.FirstOrDefault(q => q.Id == answerView.QuestionId);
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