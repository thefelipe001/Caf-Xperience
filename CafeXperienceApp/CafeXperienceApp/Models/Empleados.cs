using System.ComponentModel.DataAnnotations;

namespace CafeXperienceApp.Models
{
    public class Empleados
    {
        [Key]
        public int IdEmpleado { get; set; } // PK
        public string Nombre { get; set; }  // varchar(50), not null
        public string Cedula { get; set; }  // varchar(11), not null
        public string TandaLabor { get; set; }  // varchar(50), not null
        public decimal PorcientoComision { get; set; }  // decimal(5,2), nullable
        public DateTime FechaIngreso { get; set; }  // date, not null
        public string Estado { get; set; }  // 'A' o 'I', not null
        public int IdUsuario { get; set; }  // FK hacia la tabla Usuarios
    }
}
