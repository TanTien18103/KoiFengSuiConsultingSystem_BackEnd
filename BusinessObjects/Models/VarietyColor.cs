using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class VarietyColor
{
    public string KoiVarietyId { get; set; } = null!;

    public string ColorId { get; set; } = null!;

    public decimal? Percentage { get; set; }

    public virtual Color Color { get; set; } = null!;

    public virtual KoiVariety KoiVariety { get; set; } = null!;
}
