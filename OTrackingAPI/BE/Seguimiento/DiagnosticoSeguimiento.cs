using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Seguimiento
{
    [Table("tblDiagnosticoSeguimiento")]
    public class DiagnosticoSeguimiento
    {
        public int DiagnosticoSeguimientoId { get; set; }
        public int SeguimientoId { get; set; }
        public string CIE10Id { get; set; }
        public int? TipoControlId { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
