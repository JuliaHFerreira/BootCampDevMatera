using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BootCampDevMatera.Data;

public partial class Product
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    public virtual ICollection<OrderIten> OrderItens { get; set; } = new List<OrderIten>();
}
