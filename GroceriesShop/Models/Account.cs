namespace GroceriesShop.Models;

public partial class Account
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public int? Age { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Hashpassword { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual AccountType? Role { get; set; }

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
