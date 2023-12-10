using System;
using System.Collections.Generic;

namespace GroceriesShop.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? Name { get; set; }

    public int? Price { get; set; }

    public string? FilePath { get; set; }

    public int? CategoryId { get; set; }

    public int? SellerId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Account? Seller { get; set; }

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
