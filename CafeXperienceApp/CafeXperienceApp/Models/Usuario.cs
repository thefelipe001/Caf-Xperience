using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CafeXperienceApp.Models;

public partial class Usuario
{
    [Key]
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public int TipoUsuarioId { get; set; }

    public decimal LimiteCredito { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string Estado { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public virtual ICollection<Cafeteria> Cafeteria { get; set; } = new List<Cafeteria>();

    public virtual TipoUsuario TipoUsuario { get; set; } = null!;
}
