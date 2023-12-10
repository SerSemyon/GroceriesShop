using System;
using System.Collections.Generic;

namespace GroceriesShop.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? ProductId { get; set; }

    public int? Rating { get; set; }

    public string? TextFeedback { get; set; }

    public int? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Product? Product { get; set; }
}
