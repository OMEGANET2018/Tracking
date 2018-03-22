using System;
using System.Collections.Generic;

namespace BE.Comun
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

    public class BandejaEMO : Bandejas
    {
        public string Nombre { get; set; }
        public int EmpresaId { get; set; }
        public bool EnviarCorreo { get; set; }
        public bool DescargarArchivo { get; set; }
        public List<BandejaEMODetalle> Lista { get; set; }
    }

    public class BandejaEMODetalle
    {
        public string TipoDocumento { get; set; }
        public int TipoDocumentoId { get; set; }
        public string NroDocumento { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public string Genero { get; set; }
        public int? GeneroId { get; set; }
        public string GradoDeInstruccion { get; set; }
        public int? GradoDeInstruccionId { get; set; }
        public string PuestoLaboral { get; set; }
        public string Area { get; set; }
        public string Zona { get; set; }
        public int? ZonaId { get; set; }
        public string LugarDeTrabajo { get; set; }
        public string Discapacitado { get; set; }
        public int? DiscapacitadoId { get; set; }
        public string ProveedorClinica { get; set; }
        public string SedeRUC { get; set; }
    }
}