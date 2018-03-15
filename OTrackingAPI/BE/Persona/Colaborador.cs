using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Persona
{
    [Table("tblColaborador")]
    public class Colaborador
    {
        public int Id { get; set; }
        public int PersonaId { get; set; }
        public int? SedeId { get; set; }
        public string Direccion { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public int? GradoInstruccionId { get; set; }
        public string PuestoLaboral { get; set; }
        public string Area { get; set; }
        public int? ZonaId { get; set; }
        public string LugarDeTrabajo { get; set; }
        public int? DiscapacitadoId { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
