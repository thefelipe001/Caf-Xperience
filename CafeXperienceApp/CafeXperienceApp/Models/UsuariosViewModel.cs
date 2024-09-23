using System.ComponentModel.DataAnnotations;

namespace CafeXperienceApp.Models
{
    public class UsuariosViewModel
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

        public string? Cafeteria { get; set; } 

        public string TipoUsuario { get; set; } = null!;
    }
}
