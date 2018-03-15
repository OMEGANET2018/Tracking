using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Acceso
{
    [Table("tblUsuario")]
    public class Usuario
    {
        public int Id { get; set; }
        public int PersonaId { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
