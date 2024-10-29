namespace CafeXperienceApp.Models
{
    public class EmpleadosViemModel
    {
        public int IdEmpleado { get; set; }

        public string Nombre { get; set; } = null!;

        public string Cedula { get; set; } = null!;

        public string TandaLabor { get; set; } = null!;

        public decimal PorcientoComision { get; set; }

        public DateTime FechaIngreso { get; set; }

        public string Estado { get; set; } = null!;

        public int IdUsuario { get; set; }

    }
}
