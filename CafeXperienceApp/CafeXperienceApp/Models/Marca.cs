using System;
using System.Collections.Generic;

namespace CafeXperienceApp.Models;

public partial class Marca
{
    public int IdMarca { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string? RutaImagen { get; set; }

    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();
}
