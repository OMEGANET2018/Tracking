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

    public class BandejaPlanDeVida :Bandejas
    {
        public List<PlanesDeVida> Lista { get; set; }
    }

    public class PlanesDeVida
    {
        public int PlanDeVidaId { get; set; }
        public string Nombre { get; set; }
        public int StatusId { get; set; }
        public int UsuGraba { get; set; }
        public List<DiagnosticoPlanesDeVida> Diagnosticos { get; set; }
        public List<CitaPlanesDeVida> Citas { get; set; }
    }

    public class DiagnosticoPlanesDeVida
    {
        public int DiagnosticoId { get; set; }
        public string CIE10 { get; set; }
        public string Diagnostico { get; set; }
        public string Control { get; set; }
        public int? ControlNumero { get; set; }
        public int? ControlNumeroTipoId { get; set; }
        public int? ControlRepeticion { get; set; }
        public int? ControlRepeticionTipoId { get; set; }
        public int StatusId { get; set; }
    }

    public class CitaPlanesDeVida
    {
        public int CitaId { get; set; }
        public string Consulta { get; set; }
        public int? Numero { get; set; }
        public int? NumeroTipoId { get; set; }
        public int? Repeticion { get; set; }
        public int? RepeticionTipoId { get; set; }
        public int StatusId { get; set; }
    }

    public class BandejaProveedores : Bandejas
    {
        public string RazonSocial { get; set; }
        public string RUC { get; set; }

        public List<BandejaProveedoresLista> Lista { get; set; }
    }

    public class BandejaProveedoresLista
    {
        public int EmpresaId { get; set; }
        public string RazonSocial { get; set; }
        public string Ruc { get; set; }
        public string TipoEmpresa { get; set; }
        public int TipoEmpresaId { get; set; }
        public string Email { get; set; }
    }

    public class BandejaSeguimiento : Bandejas
    {
        public string Nombre { get; set; }
        public string NroDocumento { get; set; }
        public List<BandejaSeguimientoDetalle> Lista { get; set; }
    }

    public class BandejaSeguimientoDetalle
    {
        public int SeguimientoId { get; set; }
        public string Nombre { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoControl { get; set; }
        public int TipoSeguimientoId { get; set; }
        public string TipoSeguimiento { get; set; }
        public int StatusSeguimientoId { get; set; }
        public string StatusSeguimiento { get; set; }
        public string Color { get; set; }
    }
}