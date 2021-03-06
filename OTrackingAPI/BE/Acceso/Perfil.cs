﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Acceso
{
    [Table("tblPerfil")]
    public class Perfil
    {
        public int PerfilId { get; set; }
        public int RolId { get; set; }
        public int MenuId { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }
}
