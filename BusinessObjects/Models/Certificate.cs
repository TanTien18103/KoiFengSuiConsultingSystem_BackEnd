using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Certificate
{
    public string CertificateId { get; set; } = null!;

    public DateOnly? IssueDate { get; set; }

    public string? Description { get; set; }

    public string? CertificateImage { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<EnrollCert> EnrollCerts { get; set; } = new List<EnrollCert>();
}
