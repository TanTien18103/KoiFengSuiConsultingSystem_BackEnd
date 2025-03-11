using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Shape
{
    public string ShapeId { get; set; } = null!;

    public string? ShapeName { get; set; }

    public string? Element { get; set; }

    public virtual ICollection<KoiPond> KoiPonds { get; set; } = new List<KoiPond>();
}
