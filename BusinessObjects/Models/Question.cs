using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Question
{
    public string QuestionId { get; set; } = null!;

    public string? QuizId { get; set; }

    public string? QuestionText { get; set; }

    public string? QuestionType { get; set; }

    public DateTime? CreateAt { get; set; }

    public decimal? Point { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual Quiz? Quiz { get; set; }
}
