namespace DayOfFun.Model
{
    public class Answer
    {
        private int id;
        private int questionID;
        private int quizID;
        private int userId;
        private byte result;

        public byte Result { get => result; set => result = value; }
        public int UserId { get => userId; set => userId = value; }
        public int QuizID { get => quizID; set => quizID = value; }
        public int QuestionID { get => questionID; set => questionID = value; }
        public int Id { get => id; set => id = value; }

        public Answer()
        {
        }
    }
}
