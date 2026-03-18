using System;
using System.Collections.Generic;

namespace BootCampDevMatera.Data;

public partial class OrderIten
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string ProductCode { get; set; } = null!;

    public int Quantity { get; set; }

    public Decimal Value { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product ProductCodeNavigation { get; set; } = null!;
}
