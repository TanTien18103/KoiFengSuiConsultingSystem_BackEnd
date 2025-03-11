using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Wallet
{
    public string WalletId { get; set; } = null!;

    public string? CustomerId { get; set; }

    public decimal? Balance { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
}
