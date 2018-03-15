using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTracking.Models.Acceso
{
    public class UsuarioLogin
    {
        public int UsuarioId { get; set; }
        public int PersonaId { get; set; }
        public int EmpresaId { get; set; }
        public byte[] foto { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Contrasenia { get; set; }
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
        public int PadreId { get; set; }
        public string Icono { get; set; }
        public List<SubMenu> SubMenus { get; set; }
    }

    public class SubMenu
    {
        public int MenuId { get; set; }
        public string Descripcion { get; set; }
        public int PadreId { get; set; }
        public string Icono { get; set; }
        public string Uri { get; set; }
    }
}