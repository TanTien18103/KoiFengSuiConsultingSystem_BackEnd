using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Chapter
{
    public string ChapterId { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public TimeOnly? Duration { get; set; }

    public string? Video { get; set; }

    public string? CourseId { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<EnrollChapter> EnrollChapters { get; set; } = new List<EnrollChapter>();
}
