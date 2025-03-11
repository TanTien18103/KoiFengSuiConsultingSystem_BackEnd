using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Transaction
{
    public string TransactionId { get; set; } = null!;

    public string? TransactionType { get; set; }

    public string? DocNo { get; set; }

    public string? TransactionName { get; set; }

    public string? Status { get; set; }
}
