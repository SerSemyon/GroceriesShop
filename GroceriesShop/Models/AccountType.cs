using System;
using System.Collections.Generic;

namespace GroceriesShop.Models;

public partial class AccountType
{
    public int TypeId { get; set; }

    public string? TypeName { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
