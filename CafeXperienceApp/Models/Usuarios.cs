using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Usuarios
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int IdTipoUsuario { get; set; }
        public decimal Límite_de_Credito { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Estado { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public virtual Cafeteria Cafeteria { get; set; }

    }
}
