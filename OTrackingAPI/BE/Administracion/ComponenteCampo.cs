using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Administracion
{
    [Table("tblComponenteCampo")]
    public class ComponenteCampo
    {
        public int Id { get; set; }
        public int ServicioComponenteId { get; set; }
        public int ComponenteId { get; set; }
        public string Valor { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
