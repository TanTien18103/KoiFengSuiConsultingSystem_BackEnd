using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class RegisterAttend
{
    public string AttendId { get; set; } = null!;

    public string? WorkshopId { get; set; }

    public string? AttendName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? CustomerId { get; set; }

    public string? Status { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual WorkShop? Workshop { get; set; }
}
