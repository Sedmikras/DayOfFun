using Microsoft.Data.SqlClient;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DayOfFun.SeleniumTests;

/// <summary>
/// Rewrited UNIT tests to Selenium. Used frontend for testing parts of the application. 
/// </summary>
public class SeleniumTests
{
    IWebDriver driver;

    private SqlConnection conn;

    WebDriverWait wait;


    [OneTimeSetUp]
    public void Setup()
    {
        //Below code is to get the drivers folder path dynamically.

        //You can also specify chromedriver.exe path dircly ex: C:/MyProject/Project/drivers

        string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        //Creates the ChomeDriver object, Executes tests on Google Chrome

        driver = new EdgeDriver(path + @"\Drivers\");
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        //If you want to Execute Tests on Firefox uncomment the below code

        // Specify Correct location of geckodriver.exe folder path. Ex: C:/Project/drivers

        //driver= new FirefoxDriver(path + @"\drivers\");

        string connectString =
            "Server=(localdb)\\mssqllocaldb;Database=DayOfFun;Trusted_Connection=True;MultipleActiveResultSets=true";
        conn = new SqlConnection(connectString);
        conn.Open();
        string query = @"select * from dbo.Users where Email = 'test@test.tst'";
        SqlCommand cmd = new SqlCommand(query, conn);
        SqlDataReader dr = cmd.ExecuteReader();
        //Prepare database for testing
        if (dr.HasRows)
        {
            query = @"delete from dbo.Users where Email = 'test@test.tst'";
            cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        //Prepare testing user
        query = @"select * from dbo.Users where Email = 'net@kiv.fav'";
        cmd = new SqlCommand(query, conn);
        dr = cmd.ExecuteReader();
        if (!dr.HasRows)
        {
            query = @"insert into dbo.Users(Email, IsTemporary, Password, Username)
values ('net@kiv.fav', 0, 'test', 'tester') ";
            cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        conn.Close();
    }


    [Test]
    public void RegisterTest()
    {
        driver.Navigate().GoToUrl("https://localhost:7228/");
        Assert.IsTrue(string.Equals("https://localhost:7228/account/login", driver.Url,
            StringComparison.CurrentCultureIgnoreCase));
        IWebElement email = driver.FindElement(By.Id("Email"));
        IWebElement password = driver.FindElement(By.Id("Password"));
        IWebElement submit = driver.FindElement(By.XPath("//input[@type=\"submit\"]"));
        email.SendKeys("test@test.tst");
        password.SendKeys("test");
        submit.Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/account/login", driver.Url,
            StringComparison.CurrentCultureIgnoreCase));
        driver.FindElement(By.XPath("//li[contains(text(), \"Username or Password is wrong\")]"));
        driver.FindElement(By.XPath("//a[contains(text(), \"Register\")]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/account/register", driver.Url,
            StringComparison.CurrentCultureIgnoreCase));
        submit = driver.FindElement(By.XPath("//input[@type=\"submit\"]"));
        submit.Click();

        //All errors
        Assert.IsTrue(driver.FindElements(By.XPath("//span[contains(@class, \"field-validation-error\")]")).Count == 4);
        IWebElement username = driver.FindElement(By.Id("Username"));
        IWebElement confirmPassword = driver.FindElement(By.Id("ConfirmPassword"));
        email = driver.FindElement(By.Id("Email"));
        password = driver.FindElement(By.Id("Password"));
        username.SendKeys("test");
        Assert.IsTrue(driver.FindElements(By.XPath("//span[contains(@class, \"field-validation-error\")]")).Count == 3);
        email.SendKeys("se");
        Assert.IsTrue(driver.FindElements(By.XPath("//span[contains(@class, \"field-validation-error\")]")).Count == 3);
        Assert.IsTrue(driver
            .FindElement(By.XPath(
                "//span[@id=\"Email-error\"][contains(text(), \"The field Email must match the regular expression\")]"))
            .Displayed);
        email.Clear();
        email.SendKeys("net@kiv.fav");
        Assert.IsTrue(driver.FindElements(By.XPath("//span[contains(@class, \"field-validation-error\")]")).Count == 2);
        password.SendKeys("test");
        Assert.IsTrue(driver.FindElements(By.XPath("//span[contains(@class, \"field-validation-error\")]")).Count == 1);
        confirmPassword.SendKeys("heslo");
        Assert.IsTrue(driver.FindElements(By.XPath("//span[contains(@class, \"field-validation-error\")]")).Count == 0);
        driver.FindElement(By.XPath("//input[@type=\"submit\"]")).Click();
        Assert.IsTrue(driver.FindElement(By.XPath("//li[contains(text(), \"Passwords does not match\")]")).Displayed);

        //REDIRECT - try again - must load variables
        confirmPassword = driver.FindElement(By.Id("ConfirmPassword"));
        password = driver.FindElement(By.Id("Password"));
        confirmPassword.SendKeys("test");
        password.SendKeys("test");
        driver.FindElement(By.XPath("//input[@type=\"submit\"]")).Click();
        Assert.True(driver.FindElement(By.XPath("//div[@class=\"toast toast-error\"]")).Displayed);

        //REDIRECT - try again 
        email = driver.FindElement(By.Id("Email"));
        password = driver.FindElement(By.Id("Password"));
        confirmPassword = driver.FindElement(By.Id("ConfirmPassword"));
        email.Clear();
        email.SendKeys("test@test.tst");
        confirmPassword.SendKeys("test");
        password.SendKeys("test");
        driver.FindElement(By.XPath("//input[@type=\"submit\"]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/account/login", driver.Url,
            StringComparison.CurrentCultureIgnoreCase));
    }

    [Test]
    public void QuizCreateTest()
    {
        SqlCommand cmd = new SqlCommand("delete from dbo.Quizzes where Title like 'TST_Quiz_Advanced'", conn);
        cmd.ExecuteNonQuery();
        cmd = new SqlCommand("delete from dbo.Quizzes where Title like 'TST_Quiz_Simple'", conn);
        cmd.ExecuteNonQuery();
        Login();
        SimpleQuizCreateCase();
        AdvancedQuizCreateCaseWithAutocomplete();
        cmd = new SqlCommand("delete from dbo.Quizzes where Title like 'TST_Quiz_Advanced'", conn);
        cmd.ExecuteNonQuery();
        cmd = new SqlCommand("delete from dbo.Quizzes where Title like 'TST_Quiz_Simple'", conn);
        cmd.ExecuteNonQuery();
        Logout();
    }

    private void prepareFillResultTestQuiz()
    {
        driver.FindElement(By.XPath("//a[contains(text(), \"Add-new\")]")).Click();
        driver.FindElement(By.Id("Title")).SendKeys("Ktery predmet by mel Prema delat prednostne");
        IWebElement addButton = driver.FindElement(By.Id("btnadd"));
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        driver.FindElement(By.XPath("//div[@id=\"question_0\"]/div[@class=\"col-8\"]/input")).SendKeys("KIV/NET");
        driver.FindElement(By.XPath("//div[@id=\"question_1\"]/div[@class=\"col-8\"]/input")).SendKeys("KIV/ISW");
        driver.FindElement(By.XPath("//div[@id=\"question_2\"]/div[@class=\"col-8\"]/input")).SendKeys("KIV/VSS");
        driver.FindElement(By.XPath("//div[@id=\"question_3\"]/div[@class=\"col-8\"]/input")).SendKeys("KIV/LA");
        driver.FindElement(By.XPath("//div[@id=\"question_4\"]/div[@class=\"col-8\"]/input")).SendKeys("KPV/MNT");
        driver.FindElement(By.XPath("//div[@id=\"question_5\"]/div[@class=\"col-8\"]/input")).SendKeys("KIV/ACS1");
        driver.FindElement(By.XPath("//div[@id=\"question_6\"]/div[@class=\"col-8\"]/input")).SendKeys("KIV/FJP");
        driver.FindElement(By.XPath("//div[@id=\"question_7\"]/div[@class=\"col-8\"]/input")).SendKeys("KIV/PIA");
        driver.FindElement(By.XPath("//div[@id=\"question_8\"]/div[@class=\"col-8\"]/input")).SendKeys("KIV/PPR");
        driver.FindElement(By.XPath("//input[contains(@value, \"Create\")]")).Click();
    }

    private void Logout()
    {
        var iterator = 0;
        while (iterator < 10)
        {
            try
            {
                driver.FindElement(By.XPath("//a[@href='/Account/Logout']")).Click();
                iterator = 10;
            }
            catch (Exception e)
            {
                iterator++;
            }
        }
    }

    [Test]
    public void FillResultTest()
    {
        SqlCommand com;
        int ID;
        Login();

        try
        {
            driver.FindElement(By.XPath("//td[contains(text(), 'Ktery predmet by mel Prema delat prednostne')]"));
            com = new SqlCommand(
                "select ID from dbo.Quizzes where Title = 'Ktery predmet by mel Prema delat prednostne' and OwnerId = (select ID from dbo.Users where Email = 'net@kiv.fav')",
                conn);
            ID = Int32.Parse(com.ExecuteScalar().ToString());
            driver.FindElement(By.XPath("//a[contains(@href, '/Quiz/Delete?id=" + ID + "')]")).Click();
        }
        catch (Exception e)
        {
            
        }
        prepareFillResultTestQuiz();

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        Assert.IsTrue(string.Equals("https://localhost:7228/", driver.Url));
        com = new SqlCommand(
            "select ID from dbo.Quizzes where Title = 'Ktery predmet by mel Prema delat prednostne' and OwnerId = (select ID from dbo.Users where Email = 'net@kiv.fav')",
            conn);
        ID = Int32.Parse(com.ExecuteScalar().ToString());
        driver.FindElement(By.XPath("//a[contains(@href, '/Quiz/Fill?id=" + ID + "')]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/Quiz/Fill?id=" + ID, driver.Url));
        var answer = driver.FindElement(By.Name("QuestionAnswers[0].Result"));
        //create select element object 
        var selectElement = new SelectElement(answer);
        selectElement.SelectByText("NO");
        selectElement.SelectByText("YES");
        driver.FindElement(By.XPath("//input[@type='submit']")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/", driver.Url));
        Assert.IsTrue(driver
            .FindElement(By.XPath(
                "//td[contains(text(), 'Ktery predmet by mel Prema delat prednostne')]/following-sibling::td[contains(text(),'FINISHED')]"))
            .Displayed);
        driver.FindElement(By.XPath("//a[contains(@href, '/Quiz/Users?quizId=" + ID + "')]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/Quiz/Users?quizId=" + ID, driver.Url));
        Assert.IsTrue(driver
            .FindElement(
                By.XPath("//th[contains(text(), 'tester')]/following-sibling::th/div[contains(text(), 'FINISHED')]"))
            .Displayed);
        driver.FindElement(By.XPath("//button[contains(text(), 'Share with others')]")).Click();
        driver.FindElement(By.Id("Email")).SendKeys("pvanecek@kiv.zcu.cz");
        driver.FindElement(By.XPath("//button[contains(text(), 'Save')]")).Click();
        var iterator = 0;
        while (iterator < 10)
        {
            try
            {
                driver.FindElement(By.XPath("//a[@href='/'][contains(text(), 'Back to List')]")).Click();
                iterator = 10;
            }
            catch (Exception e)
            {
                iterator++;
            }
        }

        Assert.IsTrue(driver
            .FindElement(By.XPath(
                "//td[contains(text(), 'Ktery predmet by mel Prema delat prednostne')]/following-sibling::td[contains(text(),'WAITING')]"))
            .Displayed);

        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@href='/Account/Logout']")));
        Logout();
        driver.Navigate().GoToUrl("https://localhost:7228/Public/Fill?id=" + ID + "&email=" + "pvanecek@kiv.zcu.cz");
        answer = driver.FindElement(By.Name("QuestionAnswers[0].Result"));
        //create select element object 
        selectElement = new SelectElement(answer);
        selectElement.SelectByText("NO");
        selectElement.SelectByText("YES");
        driver.FindElement(By.XPath("//input[@type='submit']")).Click();
        driver.FindElement(By.XPath("//a[@href='/Account/Logout']")).Click();
        Login();
        Assert.IsTrue(driver
            .FindElement(By.XPath(
                "//td[contains(text(), 'Ktery predmet by mel Prema delat prednostne')]/following-sibling::td[contains(text(),'FINISHED')]"))
            .Displayed);
        driver.FindElement(By.XPath("//a[contains(@href, '/Quiz/Users?quizId=" + ID + "')]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/Quiz/Users?quizId=" + ID, driver.Url));
        Assert.IsTrue(driver
            .FindElement(
                By.XPath("//th[contains(text(), 'tester')]/following-sibling::th/div[contains(text(), 'FINISHED')]"))
            .Displayed);
        driver.FindElement(By.XPath("//button[contains(text(), 'Share with others')]")).Click();
        driver.FindElement(By.Id("Email")).SendKeys("vedouci-dp@kiv.zcu.cz");
        driver.FindElement(By.XPath("//button[contains(text(), 'Save')]")).Click();
        iterator = 0;
        while (iterator < 10)
        {
            try
            {
                driver.FindElement(By.XPath("//a[@href='/'][contains(text(), 'Back to List')]")).Click();
                iterator = 10;
            }
            catch (Exception e)
            {
                iterator++;
            }
        }

        Assert.IsTrue(driver
            .FindElement(By.XPath(
                "//td[contains(text(), 'Ktery predmet by mel Prema delat prednostne')]/following-sibling::td[contains(text(),'WAITING')]"))
            .Displayed);
        Logout();
        driver.Navigate().GoToUrl("https://localhost:7228/Public/Fill?id=" + ID + "&email=" + "vedouci-dp@kiv.zcu.cz");
        driver.FindElement(By.XPath("//button[contains(text(), 'Add Question')]")).Click();
        driver.FindElement(By.XPath("//input[@name='Text']")).SendKeys("KIV/DP");
        driver.FindElement(By.XPath("//button[contains(text(), 'Save')]")).Click();
        answer = driver.FindElement(By.Name("QuestionAnswers[9].Result"));
        //create select element object 
        selectElement = new SelectElement(answer);
        selectElement.SelectByText("YES");
        driver.FindElement(By.XPath("//input[@type='submit']")).Click();
        Logout();
        driver.Navigate().GoToUrl("https://localhost:7228/Public/Fill?id=" + ID + "&email=" + "pvanecek@kiv.zcu.cz");
        answer = driver.FindElement(By.Name("QuestionAnswers[9].Result"));
        //create select element object 
        selectElement = new SelectElement(answer);
        selectElement.SelectByValue("1");
        driver.FindElement(By.XPath("//input[@type='submit']")).Click();
        Logout();
        Login();
        Assert.IsTrue(driver
            .FindElement(By.XPath(
                "//td[contains(text(), 'Ktery predmet by mel Prema delat prednostne')]/following-sibling::td[contains(text(),'WAITING')]"))
            .Displayed);
        driver.FindElement(By.XPath("//a[contains(@href, '/Quiz/Fill?id=" + ID + "')]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/Quiz/Fill?id=" + ID, driver.Url));
        answer = driver.FindElement(By.Name("QuestionAnswers[9].Result"));
        //create select element object 
        selectElement = new SelectElement(answer);
        selectElement.SelectByValue("1");
        driver.FindElement(By.XPath("//input[@type='submit']")).Click();
        Assert.IsTrue(driver
            .FindElement(By.XPath(
                "//td[contains(text(), 'Ktery predmet by mel Prema delat prednostne')]/following-sibling::td[contains(text(),'FINISHED')]"))
            .Displayed);
        driver.FindElement(By.XPath("//a[contains(@href, '/Quiz/Details?id=" + ID + "')]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/Quiz/Details?id=" + ID, driver.Url));
        Logout();
    }

    [Test]
    public void SimpleFillQuizTest()
    {
        //PREPARE DATA
        var newUserEmail = "net-test@kiv.fav";
        int ID;
        SqlCommand com =
            new SqlCommand(
                "select ID from dbo.Quizzes where Title = 'TST_Quiz_Advanced' and OwnerId = (select ID from dbo.Users where Email = 'net@kiv.fav')",
                conn);
        var result = com.ExecuteScalar();
        if (result != null)
        {
            ID = Int32.Parse(com.ExecuteScalar().ToString());
            com = new SqlCommand("update Quizzes set State = 1 where Id=" + ID, conn);
            com.ExecuteNonQuery();
            
            com = new SqlCommand("select ID from Users where email ='" + newUserEmail + "'", conn);
            var reader = com.ExecuteReader();
            if (reader.HasRows)
            {
                String sqlcommandString = "delete from QuizUser where QuizzesId=" + ID.ToString() +
                                          " and UsersId=(select ID from Users where Email='" + newUserEmail + "')";
                com = new SqlCommand(sqlcommandString, conn);
                com.ExecuteNonQuery();
            }
        }

        Login();
        AdvancedQuizCreateCaseWithAutocomplete();
        com =
            new SqlCommand(
                "select ID from dbo.Quizzes where Title = 'TST_Quiz_Advanced' and OwnerId = (select ID from dbo.Users where Email = 'net@kiv.fav')",
                conn);
        ID = Int32.Parse(com.ExecuteScalar().ToString());
        
        
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        Assert.IsTrue(string.Equals("https://localhost:7228/", driver.Url));
        Assert.IsTrue(driver.FindElements(By.XPath("//td[contains(text(), 'TST_Quiz_Advanced')]")).Count == 1);

        driver.FindElement(By.XPath("//a[contains(@href, '/Quiz/Fill?id=" + ID + "')]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/Quiz/Fill?id=" + ID, driver.Url));
        Assert.IsTrue(driver.FindElements(By.XPath("//label[contains(text(), 'Text')]")).Count == 8);
        // select the drop down list
        var answer = driver.FindElement(By.Name("QuestionAnswers[0].Result"));
        //create select element object 
        var selectElement = new SelectElement(answer);
        selectElement.SelectByText("NO");
        selectElement.SelectByText("YES");
        driver.FindElement(By.XPath("//input[@type='submit']")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/", driver.Url));
        Assert.IsTrue(driver
            .FindElement(By.XPath(
                "//td[contains(text(), TST_Quiz_Advanced)]/following-sibling::td[contains(text(),'FINISHED')]"))
            .Displayed);
        driver.FindElement(By.XPath("//a[contains(@href, '/Quiz/Users?quizId=" + ID + "')]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/Quiz/Users?quizId=" + ID, driver.Url));
        driver.FindElement(By.XPath("//button[contains(text(), 'Share with others')]")).Click();
        driver.FindElement(By.Id("Email")).SendKeys("net@kiv.fav");
        driver.FindElement(By.XPath("//button[contains(text(), 'Save')]")).Click();
        IWebElement element =
            wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[contains(text(), 'Share with others')]")));
        var iterator = 0;
        while (iterator < 10)
        {
            try
            {
                driver.FindElement(By.XPath("//button[contains(text(), 'Share with others')]")).Click();
                iterator = 10;
            }
            catch (Exception e)
            {
                iterator++;
            }
        }

        wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal-dialog")));
        Assert.IsTrue(driver.FindElement(By.ClassName("modal-dialog")).Displayed);
        driver.FindElement(By.Id("Email")).SendKeys("");
        driver.FindElement(By.XPath("//button[contains(text(), 'Save')]")).Click();
        Assert.IsTrue(driver.FindElement(By.ClassName("modal-dialog")).Displayed);
        /*Assert.IsTrue(driver.FindElement(By.ClassName("toast-message")).Text == "Email is required");*/
        driver.FindElement(By.Id("Email")).SendKeys("abc");
        driver.FindElement(By.XPath("//button[contains(text(), 'Save')]")).Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("toast-message")));
        Assert.IsTrue(driver.FindElement(By.ClassName("modal-dialog")).Displayed);
        driver.FindElement(By.Id("Email")).Clear();
        driver.FindElement(By.Id("Email")).SendKeys(newUserEmail);
        driver.FindElement(By.XPath("//button[contains(text(), 'Save')]")).Click();
        Assert.IsTrue(driver.FindElement(By.XPath("//th[contains(text(), '" + newUserEmail + "')]")).Displayed);
        Assert.IsTrue(driver
            .FindElement(By.XPath(
                "//th[contains(text(), '" + newUserEmail +
                "')]/following-sibling::th/div[contains(text(),'NOT STARTED')]"))
            .Displayed);
        driver.FindElement(By.XPath("//a[@href='/'][contains(text(), 'Back to List')]")).Click();
        Assert.IsTrue(driver
            .FindElement(By.XPath(
                "//td[contains(text(), TST_Quiz_Advanced)]/following-sibling::td[contains(text(),'WAITING')]"))
            .Displayed);
        driver.Navigate().GoToUrl("https://localhost:7228/Public/Fill?id=" + ID + "&email=" + newUserEmail);
        // select the drop down list
        answer = driver.FindElement(By.Name("QuestionAnswers[0].Result"));
        //create select element object 
        selectElement = new SelectElement(answer);
        selectElement.SelectByText("NO");
        selectElement.SelectByText("YES");
        driver.FindElement(By.XPath("//input[@type='submit']")).Click();
        driver.Navigate().GoToUrl("https://localhost:7228/");
        Assert.IsTrue(driver
            .FindElement(By.XPath(
                "//td[contains(text(), TST_Quiz_Advanced)]/following-sibling::td[contains(text(),'FINISHED')]"))
            .Displayed);
        driver.FindElement(By.XPath("//a[contains(@href, '/Quiz/Details?id=" + ID + "')]")).Click();
        Assert.IsTrue(driver.FindElements(By.ClassName("questionText")).Count == 1);
        Assert.IsTrue(string.Equals("2", driver.FindElement(By.ClassName("score")).Text));
        iterator = 0;
        while (iterator < 10)
        {
            try
            {
                driver.FindElement(By.XPath("//a[@href='/'][contains(text(), 'Back to List')]")).Click();
                iterator = 10;
            }
            catch (Exception e)
            {
                iterator++;
            }
        }
        driver.FindElement(By.XPath("//a[contains(@href, '/Quiz/Delete?id=" + ID + "')]")).Click();
        Logout();
    }

    private void Login()
    {
        driver.Navigate().GoToUrl("https://localhost:7228/");
        Assert.IsTrue(string.Equals("https://localhost:7228/account/login", driver.Url,
            StringComparison.CurrentCultureIgnoreCase));
        IWebElement email = driver.FindElement(By.Id("Email"));
        IWebElement password = driver.FindElement(By.Id("Password"));
        IWebElement submit = driver.FindElement(By.XPath("//input[@type=\"submit\"]"));
        email.SendKeys("net@kiv.fav");
        password.SendKeys("test");
        submit.Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/", driver.Url,
            StringComparison.CurrentCultureIgnoreCase));
    }

    private void AdvancedQuizCreateCaseWithAutocomplete()
    {
        driver.FindElement(By.XPath("//a[contains(text(), \"Add-new\")]")).Click();
        driver.FindElement(By.Id("Title")).SendKeys("TST_Quiz_Advanced");
        IWebElement elem = driver.FindElement(By.XPath("//input[contains(@name, \"Questions[0].Text\")]"));
        //TODO - pokud zbude hodně času (nezbude), mrknout na testování autocomplete
        /*
        elem.SendKeys("Je");
        Assert.True(driver.FindElements(By.XPath("//ul/li/div[contains(text(), \"Jednoduch\")]")).Count != 0);
        driver.FindElement(By.XPath("//ul/li/div[contains(text(), \"Jednoduchá odpověď\")][@class=\"ui-menu-item-wrapper\"]")).Click();
        Assert.True(elem.Text == "Jednoduchá odpověď");
        */
        Assert.IsFalse(driver.FindElement(By.XPath("//div[@id=\"question_0\"]/div[@class=\"col-4\"]/button"))
            .Displayed);
        IWebElement addButton = driver.FindElement(By.Id("btnadd"));
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        addButton.Click();
        driver.FindElement(By.XPath("//input[contains(@value, \"Create\")]")).Click();
        Assert.IsTrue(driver.FindElement(By.XPath("//span[contains(text(), \"Question text is required\")]"))
            .Displayed);
        driver.FindElement(By.XPath("//div[@id=\"question_3\"]/div[@class=\"col-8\"]/input")).SendKeys("Text3");
        driver.FindElement(By.XPath("//div[@id=\"question_4\"]/div[@class=\"col-8\"]/input")).SendKeys("Text4");
        driver.FindElement(By.XPath("//div[@id=\"question_6\"]/div[@class=\"col-8\"]/input")).SendKeys("Text6");
        driver.FindElement(By.XPath("//div[@id=\"question_7\"]/div[@class=\"col-8\"]/input")).SendKeys("Text7");
        driver.FindElement(By.XPath("//div[@id=\"question_8\"]/div[@class=\"col-8\"]/input")).SendKeys("Text8");
        driver.FindElement(By.XPath("//div[@id=\"question_9\"]/div[@class=\"col-8\"]/input")).SendKeys("Text9");
        driver.FindElement(By.XPath("//div[@id=\"question_10\"]/div[@class=\"col-8\"]/input")).SendKeys("Text10");
        driver.FindElement(By.XPath("//div[@id=\"question_11\"]/div[@class=\"col-8\"]/input")).SendKeys("Text11");
        Assert.IsFalse(driver.FindElement(By.XPath("//div[@id=\"question_0\"]/div[@class=\"col-4\"]/button"))
            .Displayed);
        driver.FindElement(By.XPath("//input[contains(@value, \"Create\")]")).Click();
        Assert.IsTrue(driver.FindElement(By.XPath("//span[contains(text(), \"Question text is required\")]"))
            .Displayed);
        driver.FindElement(By.XPath("//div[@id=\"question_2\"]/div[@class=\"col-4\"]/button")).Click();
        driver.FindElement(By.XPath("//div[@id=\"question_1\"]/div[@class=\"col-4\"]/button")).Click();
        driver.FindElement(By.XPath("//input[contains(@value, \"Create\")]")).Click();
        Assert.IsTrue(driver.FindElement(By.XPath("//div/ul/li[contains(text(), \"Question text is required\")]"))
            .Displayed);
        driver.FindElement(By.XPath("//div[@id=\"question_0\"]/div[@class=\"col-8\"]/input")).SendKeys("Text0");
        driver.FindElement(By.XPath("//input[contains(@value, \"Create\")]")).Click();
        Assert.IsTrue(driver.FindElement(By.XPath("//div/ul/li[contains(text(), \"Question text is required\")]"))
            .Displayed);
        driver.FindElement(By.XPath("//div[@id=\"question_3\"]/div[@class=\"col-8\"]/input")).SendKeys("Text5");
        var iterator = 0;
        while (iterator < 2)
        {
            try
            {
                driver.FindElement(By.XPath("//input[contains(@value, \"Create\")]")).Click();
                if (!string.Equals("https://localhost:7228/", driver.Url))
                {
                    continue;
                }

                iterator = 2;
            }
            catch (Exception e)
            {
                iterator++;
            }
        }


        Assert.IsTrue(string.Equals("https://localhost:7228/", driver.Url));
    }

    private void SimpleQuizCreateCase()
    {
        driver.FindElement(By.XPath("//a[contains(text(), \"Add-new\")]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/quiz/create", driver.Url,
            StringComparison.CurrentCultureIgnoreCase));
        driver.FindElement(By.XPath("//a[contains(text(), \"Show all\")]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/", driver.Url,
            StringComparison.CurrentCultureIgnoreCase));
        driver.FindElement(By.XPath("//a[contains(text(), \"Add-new\")]")).Click();
        driver.FindElement(By.XPath("//input[contains(@value, \"Create\")]")).Click();
        Assert.IsTrue(driver.FindElements(By.XPath("//span[contains(@class, \"field-validation-error\")]")).Count == 2);
        driver.FindElement(By.Id("Title")).SendKeys("TST_Quiz_Simple");
        Assert.IsTrue(driver.FindElements(By.XPath("//span[contains(@class, \"field-validation-error\")]")).Count == 1);
        driver.FindElement(By.XPath("//input[contains(@name, \"Questions[0].Text\")]")).SendKeys("Jednoduchá odpověď");
        Assert.IsTrue(driver.FindElements(By.XPath("//span[contains(@class, \"field-validation-error\")]")).Count == 0);
        driver.FindElement(By.XPath("//input[contains(@value, \"Create\")]")).Click();
        Assert.IsTrue(string.Equals("https://localhost:7228/", driver.Url,
            StringComparison.CurrentCultureIgnoreCase));
    }
}