﻿using System.ComponentModel.DataAnnotations;
using DayOfFun.Model;

namespace DayOfFun.Models;

public class Quizzes_Quesitons
{
    [Key] public int Id { get; set; }
    public int QuizID { get; set; }
    public Quiz quiz;
    public int quesitonId { get; set; }
    public Question Question;
}