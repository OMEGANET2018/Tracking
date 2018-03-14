using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Administracion
{
    [Table("tblComponente")]
    public class Componente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int PadreId { get; set; }
        public int? TipoValorId { get; set; }
        public int? ValorMinimo { get; set; }
        public int? ValorMaximo { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
