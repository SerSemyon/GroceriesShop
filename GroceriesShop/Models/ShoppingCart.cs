﻿namespace GroceriesShop.Models;

public partial class ShoppingCart
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Product? Product { get; set; }
}
