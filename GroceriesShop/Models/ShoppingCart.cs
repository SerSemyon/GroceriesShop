using System;
using System.Collections.Generic;

namespace GroceriesShop.Models;

public partial class ShoppingCart
{
    public int ShoppingCartId { get; set; }

    public int? ProductId { get; set; }

    public int? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Product? Product { get; set; }
}
