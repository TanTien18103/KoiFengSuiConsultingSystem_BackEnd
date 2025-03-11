using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class EnrollChapter
{
    public string EnrollChapterId { get; set; } = null!;

    public string? ChapterId { get; set; }

    public string? Status { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual ICollection<RegisterCourse> RegisterCourses { get; set; } = new List<RegisterCourse>();
}
