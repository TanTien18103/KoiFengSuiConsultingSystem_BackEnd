using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Contract
{
    public string ContractId { get; set; } = null!;

    public string? Status { get; set; }

    public string? DocNo { get; set; }

    public string? ContractName { get; set; }

    public virtual ICollection<BookingOffline> BookingOfflines { get; set; } = new List<BookingOffline>();
}
