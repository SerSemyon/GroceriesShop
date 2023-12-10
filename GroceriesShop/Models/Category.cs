using System;
using System.Collections.Generic;

namespace GroceriesShop.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? Name { get; set; }

    public string? FilePath { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
