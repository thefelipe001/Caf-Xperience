﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TiposUsuarios
    {
        [Key]
        public int Id { get; set; }
        public string Descripción { get; set; }
        public string Estado { get; set; }
    }
}
