using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DayOfFun.Models;

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
        [StringLength(255, MinimumLength = 1, ErrorMessage ="Title must be between 1 and 255 chars")]

    public String Title { get; set; }
        public State State { get; set; }
        
        public String? Tags;

        //RELATIONSHIPS
        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        [Column]
        public User Owner;

        public virtual HashSet<User>? WaitingUsers { get; set; } = new HashSet<User>();

        //M:N        
        public virtual List<Quizzes_Users> Quizzes_Users { get; set; } = new List<Quizzes_Users>();


        [NotMapped] public virtual HashSet<User>? Users { get; set; } = new HashSet<User>();


        [NotMapped] public List<Question> ViewCollection { get; set; } = new List<Question>();

        // private attributes
        private List<Question>? _questions;

        private Dictionary<Question, HashSet<Answer>> resultset = new Dictionary<Question, HashSet<Answer>>();

        public Quiz()
        {
            this.Title = "UKNOWN";
            this.State = State.CREATED;
        }

        public virtual List<Question>? Questions
        {
            get => _questions;
            set
            {
                if (State == State.CREATED)
                {
                    _questions = value;
                    State = State.PREPARED;
                    WaitingUsers = new HashSet<User> {Owner};
                    Users = new HashSet<User>() {Owner};
                    foreach (var question in value)
                    {
                        resultset.Add(question, new HashSet<Answer>());
                    }
                }
                else if (State == State.WAITING || State == State.FINISHED)
                {
                    //_questions.UnionWith(value);
                    State = State.WAITING;
                    WaitingUsers.UnionWith(Users);
                    foreach (var question in value)
                    {
                        resultset.Add(question, new HashSet<Answer>());
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public void AddQuestions(List<Question> questions, HashSet<User> users)
        {
            Questions = questions;
            foreach (var user in users)
            {
                WaitingUsers.Add(user);
                Users.Add(user);
            }
        }

        public void AddQuestions(List<Question> questions)
        {
            Questions = questions;
            WaitingUsers = Users;
        }

        public bool isValid()
        {
            switch (State)
            {
                case State.CREATED:
                {
                    if (_questions == null)
                        return true;
                    return false;
                }
                case State.PREPARED:
                {
                    return false;
                    foreach (var question in _questions)
                    {
                        /*TODO get answers for question - should be empty*/
                    }

                    break;
                }
                case State.WAITING:
                {
                    return false;
                    break;
                    /**
                     * TODO waiting for answers from some of users
                    */
                }
                case State.FINISHED:
                {
                    return false;
                    /*
                     * TODO - answers for all users and all questions
                     */
                    break;
                }
                case State.DONE:
                {
                    return false;
                    /**
                     * TODO - results have been showed
                     */
                    break;
                }
                case State.ARCHIVED: return false;
            }

            return false;
        }

        public void Update(HashSet<Answer> answers, User user)
        {
            Boolean IsCompleted = false;
            if (State == State.PREPARED || State == State.WAITING)
            {
                foreach (var answer in answers)
                {
                    Question q = _questions.Where(q => q.Id == answer.QuestionId).FirstOrDefault();
                    resultset[q].Add(answer);
                }

                CheckUsers(user);
            }

            if (State == State.WAITING)
            {
            }

            foreach (var answer in answers)
            {
            }
        }

        private void CheckUsers(User user)
        {
            Boolean t = true;
            foreach (var pair in resultset)
            {
                if (pair.Value.Where(answer => answer.UserId == user.Id) == null)
                {
                    t = false;
                }
            }

            if (t)
            {
                WaitingUsers.Remove(user);
            }

            if (WaitingUsers.Count == 0)
            {
                State = State.FINISHED;
            }
            else
            {
                State = State.WAITING;
            }
        }

        public void addQuestion(Question Question, User user, Answer answer)
        {
            if (State == State.WAITING)
            {
                resultset.Add(Question, new HashSet<Answer>() {answer});
                Update(new HashSet<Answer>() {answer}, user);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}