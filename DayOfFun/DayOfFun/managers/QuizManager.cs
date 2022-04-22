using DayOfFun.Model;

namespace WebApplication1.managers;

public class QuizManager
{
    private List<Quiz> quizzes;

    public void resolveChanges(Quiz update)
    {
        /*TODO get quiz from DB*/
        if (quizzes.Find(q => q.Id == update.Id) != null)
        {
            if (update.isValid() && (update.State == State.PREPARED || update.State == State.WAITING))
            {
                /*TODO - update quiz and save results*/
            }
        }
        else
        {
            if(update.isValid());
                quizzes.Add(update);
        }
    }

}