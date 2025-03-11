using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class EnrollCert
{
    public string EnrollCertId { get; set; } = null!;

    public string? CertificateId { get; set; }

    public DateOnly? FinishDate { get; set; }

    public string? CustomerId { get; set; }

    public virtual Certificate? Certificate { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<RegisterCourse> RegisterCourses { get; set; } = new List<RegisterCourse>();
}
