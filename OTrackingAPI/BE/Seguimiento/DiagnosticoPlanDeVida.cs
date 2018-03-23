using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Seguimiento
{
    [Table("tblDiagnosticoPlanesDeVida")]
    public class DiagnosticoPlanDeVida
    {
        [Key]
        public int DiagnosticoPlanesDeVidaId { get; set; }
        public int PlanesDeVidaId { get; set; }
        public string CIE10Id { get; set; }
        public string Diagnostico { get; set; }
        public string Control { get; set; }
        public int? ControlNumero { get; set; }
        public int? ControlNumeroTipoId { get; set; }
        public int? ControlRepeticion { get; set; }
        public int? ControlRepeticionTipoId { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
