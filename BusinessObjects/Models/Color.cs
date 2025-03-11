using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Color
{
    public string ColorId { get; set; } = null!;

    public string? ColorName { get; set; }

    public string? ColorCode { get; set; }

    public string? Element { get; set; }

    public virtual ICollection<VarietyColor> VarietyColors { get; set; } = new List<VarietyColor>();
}
