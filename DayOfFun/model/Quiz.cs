using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace DayOfFun.Model
{
    public enum State
    {
        CREATED,
        PREPARED,
        DONE,
        WAITING,
        FINISHED,
        ARCHIVED
    }

    public class Quiz
    {
        public int Id { get; set; }

        private List<Question>? _questions;

        public List<Question>? Questions
        {
            get => _questions;
            set
            {
                if (State == State.CREATED)
                {
                    _questions = value;
                    State = State.PREPARED;
                    WaitingUsers = new List<User>() {Owner};
                    foreach (var question in value)
                    {
                        resultset.Add(question, new List<Answer>());
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private Dictionary<Question, List<Answer>> resultset = new Dictionary<Question, List<Answer>>();

        public String Title { get; set; }

        public User Owner;

        public String Tags;

        public List<User>? WaitingUsers { get; set; }

        public List<User>? Users { get; set; }

        public State State { get; set; }

        public Quiz()
        {
            this.Title = "UKNOWN";
            this.State = State.CREATED;
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

        public void Update(List<Answer> answers)
        {
            Boolean IsCompleted = false;
            if (State == State.PREPARED || State == State.WAITING)
            {
                foreach (var answer in answers)
                {
                    Question q = _questions.Find(q => q.Id == answer.Id);
                    resultset[q].Add(answer);
                }
                CheckUsers();
            }

            if (State == State.WAITING)
            {

            }

            foreach (var answer in answers)
            {

            }
        }
        
        private void CheckUsers()
        {
            foreach (var pair in resultset)
            {
                if (pair.Value.TrueForAll(v => WaitingUsers.Find(u => u.Id == v.Id) != null))
                {
                    State = State.FINISHED;
                }
                else
                {
                    State = State.WAITING;
                }
            }
        }
        
        public void addQuestion(Question Question, User user, Answer answer)
        {
            if (State == State.WAITING)
            {
                resultset.Add(Question, new List<Answer>() {answer});
                Update(new List<Answer>() {answer});
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
