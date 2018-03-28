using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Administracion
{
    [Table("tblDiagnostico")]
    public class Diagnostico
    {
        public int DiagnosticoId { get; set; }
        public string CIE10 { get; set; }
        public string Nombre { get; set; }
    }
}
