using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Administracion
{
    [Table("tblServicio")]
    public class Servicio
    {
        public int ServicioId { get; set; }
        public int ColaboradorId { get; set; }
        public DateTime? FechaExamen { get; set; }
        public int TipoExamenId { get; set; }
        public int? AptitudId { get; set; }
        public int? ProoveedorId { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
