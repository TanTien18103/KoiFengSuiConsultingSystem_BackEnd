using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public string? CustomerId { get; set; }

    public string? ServiceId { get; set; }

    public string? ServiceType { get; set; }

    public decimal? Amount { get; set; }

    public string? OrderCode { get; set; }

    public string? PaymentReference { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string PaymentId { get; set; } = null!;

    public string Note { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Customer? Customer { get; set; }
}
