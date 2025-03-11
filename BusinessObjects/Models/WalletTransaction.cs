using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class WalletTransaction
{
    public string WalletTransactionId { get; set; } = null!;

    public string? WalletId { get; set; }

    public string? TransactionType { get; set; }

    public decimal? Amount { get; set; }

    public string? Status { get; set; }

    public DateTime? RequestDate { get; set; }

    public DateTime? ProcessDate { get; set; }

    public string? Description { get; set; }

    public virtual Wallet? Wallet { get; set; }
}
