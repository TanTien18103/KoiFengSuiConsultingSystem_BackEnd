using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Membership
{
    public string MembershipId { get; set; } = null!;

    public string? MembershipName { get; set; }

    public DateOnly? Limit { get; set; }

    public decimal? Discount { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
