using System;
using System.Collections.Generic;

namespace BootCampDevMatera.Data;

public partial class Order
{
    public int Id { get; set; }

    public Decimal Value { get; set; }

    public DateTime DateOrder { get; set; }

    public int IdClient { get; set; }

    public int IdSeller { get; set; }

    public virtual Client IdClientNavigation { get; set; } = null!;

    public virtual Seller IdSellerNavigation { get; set; } = null!;

    public virtual ICollection<OrderIten> OrderItens { get; set; } = new List<OrderIten>();
}
