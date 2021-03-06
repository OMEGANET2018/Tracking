﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Acceso
{
    [Table("tblUsuario")]
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public int PersonaId { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasenia { get; set; }
        public string PreguntaSecreta { get; set; }
        public string RespuestaSecreta { get; set; }
        public DateTime? FechaCaduca { get; set; }
        public int RolId { get; set; }
        public int EmpresaId { get; set; }

        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
    }

    public class UsuarioAutorizado
    {
        public int UsuarioId { get; set; }
        public int PersonaId { get; set; }
        public int EmpresaId { get; set; }
        public byte[] foto { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime? FechaCaduca { get; set; }
        public int RolId { get; set; }
        public string Rol { get; set; }
        public List<Autorizacion> Autorizacion { get; set; }
    }

    public class Autorizacion
    {
        public int PerfilId { get; set; }
        public int RolId { get; set; }
        public int MenuId { get; set; }
        public string Descripcion { get; set; }
        public int? PadreId { get; set; }
        public string Icono { get; set; }
        public string Uri { get; set; }
        public List<SubMenu> SubMenus { get; set; }
    }

    public class SubMenu
    {
        public int MenuId { get; set; }
        public string Descripcion { get; set; }
        public int? PadreId { get; set; }
        public string Icono { get; set; }
        public string Uri { get; set; }
    }
}
