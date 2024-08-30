using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Cafeteria
    {
        [Key]
        public int Id { get; set; }
        public string Descripción { get; set; }
        public string Campus { get; set; }
        public string Estado { get; set; }
        public int Campus_Id { get; set; }
        public int Encargado_id { get; set; }
        public IEnumerable<Campus> Campuses { get; set; }
        public IEnumerable<Usuarios> Usuarios { get; set; }
    }
}




