using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Seguimiento
{
    [Table("tblCita")]
    public class Cita
    {
        public int CitaId { get; set; }
        public int PlanesDeVidaId { get; set; }
        public string Consulta { get; set; }
        public int? Numero { get; set; }
        public int? NumeroTipoId { get; set; }
        public int? Repeticion { get; set; }
        public int? RepeticionTipoId { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
