using System;
using System.Collections.Generic;

namespace CafeXperienceApp.Models;

public partial class Proveedore
{
    public int IdProveedor { get; set; }

    public string NombreComercial { get; set; } = null!;

    public string Rnc { get; set; } = null!;

    public DateOnly FechaRegistro { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();
}
