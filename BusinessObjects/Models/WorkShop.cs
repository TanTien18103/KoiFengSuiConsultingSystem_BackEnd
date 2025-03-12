using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class WorkShop
{
    public string WorkshopId { get; set; } = null!;

    public string? WorkshopName { get; set; }

    public DateTime? StartDate { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public string? MasterId { get; set; }

    public int? Capacity { get; set; }

    public string? Status { get; set; }

    public decimal? Price { get; set; }

    public bool? Trending { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Master? Master { get; set; }

    public virtual ICollection<RegisterAttend> RegisterAttends { get; set; } = new List<RegisterAttend>();
}
