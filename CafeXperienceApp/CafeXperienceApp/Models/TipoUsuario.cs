using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CafeXperienceApp.Models;

public partial class TipoUsuario
{
    [Key]
    public int IdTipoUsuarios { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
