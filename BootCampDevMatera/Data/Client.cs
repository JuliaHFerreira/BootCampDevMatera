using System;
using System.Collections.Generic;

namespace BootCampDevMatera.Data;

public partial class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public DateOnly DateBirth { get; set; }

    public DateOnly Registration { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
