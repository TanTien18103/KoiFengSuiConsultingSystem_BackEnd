using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class ConsultationPackage
{
    public string ConsultationPackageId { get; set; } = null!;

    public string? PackageName { get; set; }

    public decimal? PackagePrice { get; set; }

    public virtual ICollection<BookingOffline> BookingOfflines { get; set; } = new List<BookingOffline>();
}
