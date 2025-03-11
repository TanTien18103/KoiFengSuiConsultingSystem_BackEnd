using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class EnrollQuiz
{
    public string EnrollQuizId { get; set; } = null!;

    public string? QuizId { get; set; }

    public string? EnrollAnswerId { get; set; }

    public decimal? Point { get; set; }

    public string? ParticipantId { get; set; }

    public virtual EnrollAnswer? EnrollAnswer { get; set; }

    public virtual Customer? Participant { get; set; }

    public virtual Quiz? Quiz { get; set; }

    public virtual ICollection<RegisterCourse> RegisterCourses { get; set; } = new List<RegisterCourse>();
}
