﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Chapter
{
    public string ChapterId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Video { get; set; }

    public string CourseId { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual Course Course { get; set; }

    public virtual ICollection<EnrollChapter> EnrollChapters { get; set; } = new List<EnrollChapter>();
}