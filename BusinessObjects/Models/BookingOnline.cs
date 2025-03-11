using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class BookingOnline
{
    public string BookingOnlineId { get; set; } = null!;

    public string? LinkMeet { get; set; }

    public string? Status { get; set; }

    public string MasterId { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? BookingDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? MasterNote { get; set; }

    public string? MasterScheduleId { get; set; }

    public decimal? Price { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Master Master { get; set; } = null!;

    public virtual MasterSchedule? MasterSchedule { get; set; }
}
