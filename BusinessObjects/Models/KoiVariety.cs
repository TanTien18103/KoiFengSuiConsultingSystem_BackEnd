using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class KoiVariety
{
    public string KoiVarietyId { get; set; } = null!;

    public string? Description { get; set; }

    public string? VarietyName { get; set; }

    public virtual ICollection<VarietyColor> VarietyColors { get; set; } = new List<VarietyColor>();
}
