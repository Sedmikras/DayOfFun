﻿using DayOfFun.Data.Services.Contract;
using DayOfFun.Model;
using DayOfFun.Models;
using SQLitePCL;

namespace DayOfFun.Data.Services;

public class QuestionService : IQuestionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuestionService> _logger;
    
    public QuestionService(ApplicationDbContext context, ILogger<QuestionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public void AddQuestionForQuiz(Question appendedQuestion, Quiz quiz)
    {
        if (quiz.Id == 0)
        {
            _logger.LogError("Quiz ID cannot be null");
            return;
        }

        if (_context.Questions.Where(question => question.Text == appendedQuestion.Text).Any())
        {
            _logger.LogInformation("Question with text " + appendedQuestion.Text + " already exists");
            Question questionFromDb = _context.Questions.Where(question => question.Text == appendedQuestion.Text).First();
            appendedQuestion.Id = questionFromDb.Id;
        }
        else
        {
            _logger.LogInformation("Question with text " + appendedQuestion.Text + " will be inserted into db");
            _context.Questions.Add(appendedQuestion);
            _context.SaveChanges();
        }
        
        Quizzes_Quesitons qq = new Quizzes_Quesitons()
        {
            QuizID = quiz.Id,
            quesitonId = appendedQuestion.Id
        };
        _context.Quizzes_Questions.Add(qq);
        _context.SaveChanges();
    }

    public List<Question> getQuestionsForQuizAndUser(Quiz quiz, User user)
    {
        List<Quizzes_Quesitons> relationTable = _context.Quizzes_Questions.Where(qq => qq.QuizID == quiz.Id).ToList();
        List<Question> questions = new List<Question>();
        foreach (var qq in relationTable)
        {
            Question q = _context.Questions.Where(question => question.Id == qq.quesitonId).FirstOrDefault();
            if(q!=null)
                questions.Add(q);
        }

        return questions;
    }
}