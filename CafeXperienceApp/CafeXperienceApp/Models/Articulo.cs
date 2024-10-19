using System.ComponentModel.DataAnnotations;

namespace CafeXperienceApp.Models
{
    public class Articulo
    {
        [Key]
        public int idArticulo { get; set; }
        public string Descripcion { get; set; }
        public int idMarca { get; set; }  // Relacionado con otra tabla
        public decimal Costo { get; set; }
        public int idProveedor { get; set; }  // Relacionado con otra tabla
        public int Existencia { get; set; }
        public char Estado { get; set; }  // 'A' para activo, 'I' para inactivo
        public string RutaImagen { get; set; }
    }
}
