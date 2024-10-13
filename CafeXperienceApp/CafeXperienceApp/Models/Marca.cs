using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CafeXperienceApp.Models;

public partial class Marca
{
    [Key]
    public int IdMarca { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Estado { get; set; } = null!;
}
