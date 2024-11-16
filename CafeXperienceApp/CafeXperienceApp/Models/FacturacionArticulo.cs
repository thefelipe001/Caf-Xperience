using System;
using System.Collections.Generic;

namespace CafeXperienceApp.Models;

public partial class FacturacionArticulo
{
    public int NoFactura { get; set; }

    public int IdEmpleado { get; set; }

    public int IdArticulo { get; set; }

    public int IdUsuario { get; set; }

    public DateTime FechaVenta { get; set; }

    public decimal MontoArticulo { get; set; }

    public int UnidadesVendidas { get; set; }

    public string? Comentario { get; set; }

    public string Estado { get; set; } = null!;

    public int IdCampus { get; set; }

    public virtual Articulo IdArticuloNavigation { get; set; } = null!;

    public virtual Empleados IdEmpleadoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
