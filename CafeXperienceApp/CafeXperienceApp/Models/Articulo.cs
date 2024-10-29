using System;
using System.Collections.Generic;

namespace CafeXperienceApp.Models;

public partial class Articulo
{
    public int IdArticulo { get; set; }

    public string Descripcion { get; set; } = null!;

    public int IdMarca { get; set; }

    public decimal Costo { get; set; }

    public int IdProveedor { get; set; }

    public int Existencia { get; set; }

    public string Estado { get; set; } = null!;

    public string? RutaImagen { get; set; }

    public virtual Marca IdMarcaNavigation { get; set; } = null!;

    public virtual Proveedore IdProveedorNavigation { get; set; } = null!;
}
