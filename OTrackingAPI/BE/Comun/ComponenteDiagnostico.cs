using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Comun
{
    [Table("tblComponenteDiagnostico")]
    public class ComponenteDiagnostico
    {
        public int Id { get; set; }
        public int ServicioComponenteId { get; set; }
        public int DiagnosticoId { get; set; }
        public string Observacion { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
