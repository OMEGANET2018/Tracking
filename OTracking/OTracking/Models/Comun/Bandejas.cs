using System.Collections.Generic;
using System;

namespace OTracking.Models.Comun
{
    public class Bandejas
    {
        public int TotalRegistros { get; set; }
        public int Index { get; set; }
        public int Take { get; set; }
    }

    public class BandejaUsuario : Bandejas
    {
        public string NombreUsuario { get; set; }
        public string NombrePersona { get; set; }

        public List<BandejaUsuarioLista> Lista { get; set; }
    }

    public class BandejaUsuarioLista
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Rol { get; set; }
        public string Empresa { get; set; }
        public string TipoEmpresa { get; set; }
    }

    public class BandejaComponente
    {
        public int ComponenteId { get; set; }
        public string Nombre { get; set; }
        public int PadreId { get; set; }
        public int? TipoValorId { get; set; }
        public string TipoValorTexto { get; set; }
        public int? ValorMinimo { get; set; }
        public int? ValorMaximo { get; set; }
        public int EsEliminado { get; set; }
        public int? UsuGraba { get; set; }
        public DateTime? FechaGraba { get; set; }
        public int? UsuActualiza { get; set; }
        public DateTime? FechaActualiza { get; set; }
        public int RecordStatus { get; set; }
        public List<BandejaComponente> Lista { get; set; }
    }

    public class BandejaGenerarPlantilla : Bandejas
    {
        public List<BandejaGenerarPlantillaLista> Lista { get; set; }
    }

    public class BandejaGenerarPlantillaLista
    {
        public string Nombre { get; set; }
    }
}