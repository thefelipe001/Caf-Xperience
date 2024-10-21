using System;
using System.Collections.Generic;

namespace CafeXperienceApp.Models;

public partial class TipoUsuario
{
    public int IdTipoUsuarios { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
