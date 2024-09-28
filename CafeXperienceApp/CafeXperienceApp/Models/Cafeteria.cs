﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CafeXperienceApp.Models;

public partial class Cafeteria
{
    [Key]
    public int IdCafeteria { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public int IdCampus { get; set; }

    public int IdEncargado { get; set; }

    public virtual Campus? IdCampusNavigation { get; set; } = null!;

    public virtual Usuario? IdEncargadoNavigation { get; set; } = null!;
}
