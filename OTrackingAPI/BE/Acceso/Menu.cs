using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Acceso
{
    [Table("tblMenu")]
    public class Menu
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int? PadreId { get; set; }
        public string Icono { get; set; }
        public string Uri { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
