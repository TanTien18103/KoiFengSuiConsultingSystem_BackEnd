using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class MasterSchedule
{
    public string MasterScheduleId { get; set; } = null!;

    public string? MasterId { get; set; }

    public DateOnly? Date { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<BookingOffline> BookingOfflines { get; set; } = new List<BookingOffline>();

    public virtual ICollection<BookingOnline> BookingOnlines { get; set; } = new List<BookingOnline>();

    public virtual Master? Master { get; set; }
}
