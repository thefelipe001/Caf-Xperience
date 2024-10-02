namespace CafeXperienceApp.Models
{
    public class CafeteriaViewModel
    {
        public int IdCafeteria { get; set; }
        public string Descripcion { get; set; }
        public string Encargado { get; set; }
        public string Campus { get; set; }
        public string Estado { get; set; }

        public int IdCampus { get; set; }

        public int IdEncargado { get; set; }
    }
}
