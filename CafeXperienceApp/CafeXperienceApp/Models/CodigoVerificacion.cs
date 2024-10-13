namespace CafeXperienceApp.Models
{
    public class CodigoVerificacion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; } // Relación con la tabla de Usuarios
        public string? Codigo { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public bool EsActivo { get; set; } = true;

        // Propiedad de navegación para la relación con la tabla Usuario
        public Usuario? Usuario { get; set; }
    }

}
