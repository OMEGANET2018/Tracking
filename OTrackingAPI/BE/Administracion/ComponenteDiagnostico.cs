﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Administracion
{
    [Table("tblComponenteDiagnostico")]
    public class ComponenteDiagnostico
    {
        public int ComponenteDiagnosticoId { get; set; }
        public int ServicioComponenteId { get; set; }
        public string CIE10Id { get; set; }
        public string Observacion { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
