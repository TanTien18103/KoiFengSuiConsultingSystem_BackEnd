using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class EnrollAnswer
{
    public string EnrollAnswerId { get; set; } = null!;

    public string? AnswerId { get; set; }

    public bool? Correct { get; set; }

    public virtual Answer? Answer { get; set; }

    public virtual ICollection<EnrollQuiz> EnrollQuizzes { get; set; } = new List<EnrollQuiz>();
}
