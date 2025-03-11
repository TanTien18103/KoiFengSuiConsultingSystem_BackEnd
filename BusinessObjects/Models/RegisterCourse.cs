using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class RegisterCourse
{
    public string EnrollCourseId { get; set; } = null!;

    public string? CourseId { get; set; }

    public string? EnrollCertId { get; set; }

    public string? EnrollChapterId { get; set; }

    public string? EnrollQuizId { get; set; }

    public decimal? Percentage { get; set; }

    public string? Status { get; set; }

    public virtual Course? Course { get; set; }

    public virtual EnrollCert? EnrollCert { get; set; }

    public virtual EnrollChapter? EnrollChapter { get; set; }

    public virtual EnrollQuiz? EnrollQuiz { get; set; }
}
