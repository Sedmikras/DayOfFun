using System.ComponentModel.DataAnnotations.Schema;

namespace DayOfFun.Model
{
    public class User
    {
        [ForeignKey("UserId")]
        public int Id { get; set; }

        public string Name { get; set; }

        public String Email { get; set; }

        public User()
        {
            this.Name = "UNKNOWN";
            this.Email = "UNKNOWN";
        }
    }
}
