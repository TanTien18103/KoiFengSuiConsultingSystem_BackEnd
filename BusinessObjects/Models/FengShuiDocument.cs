using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class FengShuiDocument
{
    public string FengShuiDocumentId { get; set; } = null!;

    public string? Version { get; set; }

    public string? Status { get; set; }

    public string? DocNo { get; set; }

    public string? DocumentName { get; set; }

    public virtual ICollection<BookingOffline> BookingOfflines { get; set; } = new List<BookingOffline>();
}
