using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class BookingOffline
{
    public string BookingOfflineId { get; set; } = null!;

    public string? CustomerId { get; set; }

    public string? MasterId { get; set; }

    public string? ConsultationPackageId { get; set; }

    public string? ContractId { get; set; }

    public string? DocumentId { get; set; }

    public string? RecordId { get; set; }

    public string? Location { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public string? MasterScheduleId { get; set; }

    public virtual ConsultationPackage? ConsultationPackage { get; set; }

    public virtual Contract? Contract { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual FengShuiDocument? Document { get; set; }

    public virtual Master? Master { get; set; }

    public virtual MasterSchedule? MasterSchedule { get; set; }

    public virtual Attachment? Record { get; set; }
}
