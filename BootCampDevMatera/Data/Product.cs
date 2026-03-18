using System;
using System.Collections.Generic;

namespace BootCampDevMatera.Data;

public partial class Product
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double Price { get; set; }

    public int Stock { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
