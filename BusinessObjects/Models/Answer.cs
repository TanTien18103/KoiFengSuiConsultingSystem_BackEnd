using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Answer
{
    public string AnswerId { get; set; } = null!;

    public string? QuestionId { get; set; }

    public string? OptionText { get; set; }

    public string? OptionType { get; set; }

    public DateTime? CreateAt { get; set; }

    public bool? IsCorrect { get; set; }

    public virtual ICollection<EnrollAnswer> EnrollAnswers { get; set; } = new List<EnrollAnswer>();

    public virtual Question? Question { get; set; }
}
