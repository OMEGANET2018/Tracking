﻿using System.Collections.Generic;
using OTracking.Models.Administracion;

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

    public class BandejaComponente : Componente
    {
        public int RecordStatus { get; set; }
        public List<BandejaComponente> Lista { get; set; }
    }
}