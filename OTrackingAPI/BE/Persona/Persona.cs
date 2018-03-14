using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Persona
{
    [Table("tblPersona")]
    public class Persona
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public int TipoDocumentoId { get; set; }
        public string NroDocumento { get; set; }
        public int? GeneroId { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public byte?[] Foto { get; set; }
        public string LugarNacimiento { get; set; }
        public int? EstadoCivilId { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
