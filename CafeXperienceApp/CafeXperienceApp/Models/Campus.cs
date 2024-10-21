using System;
using System.Collections.Generic;

namespace CafeXperienceApp.Models;

public partial class Campus
{
    public int IdCampus { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual ICollection<Cafeteria> Cafeteria { get; set; } = new List<Cafeteria>();
}
