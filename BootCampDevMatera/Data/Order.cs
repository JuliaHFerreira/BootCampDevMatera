using System;
using System.Collections.Generic;

namespace BootCampDevMatera.Data;

public partial class Order
{
    public int Id { get; set; }

    public int IdProduct { get; set; }

    public int Quantity { get; set; }

    public double Value { get; set; }

    public DateTime DateOrder { get; set; }

    public int IdClient { get; set; }

    public int IdSeller { get; set; }

    public virtual Client IdClientNavigation { get; set; } = null!;

    public virtual Product IdProductNavigation { get; set; } = null!;

    public virtual Seller IdSellerNavigation { get; set; } = null!;
}
