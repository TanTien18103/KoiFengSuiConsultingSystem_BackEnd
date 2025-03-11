using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class CompatibilityPoint
{
    public string CompatibilityType { get; set; } = null!;

    public double? Point { get; set; }
}
