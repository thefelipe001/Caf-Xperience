namespace CafeXperienceApp.Models
{
    public class FacturacionDetalleReporte
    {
        public int NumFactura { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Monto { get; set; }
        public int Cantidad { get; set; }
        public string? Comentario { get; set; }
        public string? NombreUsuario { get; set; }
        public string? DescripcionArticulo { get; set; }
        public string? DescripcionCampus { get; set; }
        public string? RNC { get; set; }
        public string? NombreComercial { get; set; }
    }
}
