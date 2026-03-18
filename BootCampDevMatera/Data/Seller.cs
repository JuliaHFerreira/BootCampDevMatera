using System;
using System.Collections.Generic;

namespace BootCampDevMatera.Data;

public partial class Seller
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
