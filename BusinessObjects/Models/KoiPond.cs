using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class KoiPond
{
    public string KoiPondId { get; set; } = null!;

    public string? ShapeId { get; set; }

    public string? PondName { get; set; }

    public int? Size { get; set; }

    public string? Direction { get; set; }

    public virtual Shape? Shape { get; set; }
}
