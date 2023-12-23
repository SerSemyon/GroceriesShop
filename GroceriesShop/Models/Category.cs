﻿namespace GroceriesShop.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? FilePath { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
