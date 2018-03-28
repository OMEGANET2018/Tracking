using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Seguimiento
{
    [Table("tblFechaSeguimiento")]
    public class FechaSeguimiento
    {
        public int FechaSeguimientoId { get; set; }
        public int SeguimientoId { get; set; }
        public DateTime Fecha { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
