using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Comun
{
    [Table("tblDiagnostico")]
    public class Diagnostico
    {
        public int Id { get; set; }
        public string CIE10 { get; set; }
        public string Nombre { get; set; }
    }
}
