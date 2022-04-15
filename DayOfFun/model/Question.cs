namespace DayOfFun.Model
{
    public class Question
    {
        public int Id { get; set; }

        public String Text { get; set; }

        public Boolean Enabled { get; set; }

        public String Tags { get; set; }

        public Question()
        {
            this.Text = "DEFAULT";
        }
    }
}
