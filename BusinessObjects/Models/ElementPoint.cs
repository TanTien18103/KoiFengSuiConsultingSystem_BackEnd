using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class ElementPoint
{
    public string ElementType { get; set; } = null!;

    public double? Point { get; set; }
}
