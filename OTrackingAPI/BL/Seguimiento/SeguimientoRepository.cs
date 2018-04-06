using DAL;
using BE.Comun;
using BE.Administracion;
using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using BE;
using System.IO;

namespace BL.Seguimiento
{
    public class SeguimientoRepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public BandejaSeguimiento ObtenerListadoSeguimiento(BandejaSeguimiento data)
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                int grupoTipoSeguimiento = (int)Enumeradores.GrupoParametros.TipoSeguimiento;
                int grupoStatusSeguimiento = (int)Enumeradores.GrupoParametros.StatusSeguimiento;
                int grupoTipoDocumento = (int)Enumeradores.GrupoParametros.TipoDocumentos;
                int grupoTipoControl = (int)Enumeradores.GrupoParametros.TipoControl;
                string Nombre = string.IsNullOrWhiteSpace(data.Nombre) ? "" : data.Nombre;
                string NroDocumento = string.IsNullOrWhiteSpace(data.NroDocumento) ? "" : data.NroDocumento;
                int skip = (data.Index - 1) * data.Take;
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("es-PE");
                DateTime fechaActual = DateTime.UtcNow.Date;
                int statusPorConfirmar = (int)Enumeradores.StatusSeguimiento.PorConfirmar;
                int statusConfirmado = (int)Enumeradores.StatusSeguimiento.Confirmado;
                int statusVencido = (int)Enumeradores.StatusSeguimiento.Vencido;

                var Vencidos = (from a in ctx.Seguimientos
                                where
                                a.Fecha < fechaActual &&
                                (a.StatusSeguimiento == statusPorConfirmar || a.StatusSeguimiento == statusConfirmado) &&
                                a.EsEliminado == NoEliminado
                                select a).ToList();

                foreach(var V in Vencidos)
                {
                    V.StatusSeguimiento = statusVencido;
                }

                ctx.SaveChanges();


                var Lista = (from a in ctx.Seguimientos
                             join y in ctx.DiagnosticosSeguimiento on a.SeguimientoId equals y.SeguimientoId
                             join z in ctx.Colaboradores on a.ColaboradorId equals z.ColaboradorId
                             join c in ctx.Personas on z.PersonaId equals c.PersonaId
                             join d in ctx.Parametros on new {a = grupoTipoSeguimiento, b = a.TipoSeguimiento} equals new {a = d.GrupoId, b = d.ParametroId}
                             join e in ctx.Parametros on new { a = grupoStatusSeguimiento, b = a.StatusSeguimiento } equals new { a = e.GrupoId, b = e.ParametroId }
                             join f in ctx.Parametros on new { a = grupoTipoDocumento, b = c.TipoDocumentoId } equals new { a = f.GrupoId, b = f.ParametroId }
                             join t in ctx.Parametros on new { a = grupoTipoControl, b = y.TipoControlId } equals new { a = t.GrupoId, b = (int?)t.ParametroId } into TP
                             from p in TP.DefaultIfEmpty()
                             where
                             (a.EsEliminado == NoEliminado && c.EsEliminado == NoEliminado) &&
                             (c.Nombres + " " + c.ApellidoPaterno + " " + c.ApellidoMaterno).Contains(Nombre) &&
                             c.NroDocumento.Contains(NroDocumento)
                             select new BandejaSeguimientoDetalle()
                             { 
                                SeguimientoId = a.SeguimientoId,
                                Nombre = c.Nombres + " " + c.ApellidoPaterno + " " + c.ApellidoMaterno,
                                TipoDocumento = f.Valor1,
                                NroDocumento = c.NroDocumento,
                                Fecha = a.Fecha,
                                TipoSeguimientoId = a.TipoSeguimiento,
                                TipoSeguimiento = d.Valor1,
                                StatusSeguimientoId = a.StatusSeguimiento,
                                StatusSeguimiento = e.Valor1,
                                Color = e.Valor2,
                                TipoControl = p.Valor1
                             }).ToList();

                foreach (var L in Lista)
                {
                    L.Fecha = DateTime.Parse(L.Fecha.ToString("yyyy-MM-ddTHH:mm:ss"),CultureInfo.CurrentCulture,DateTimeStyles.AssumeUniversal);
                }

                int TotalRegistros = Lista.Count;

                if (data.Take > 0)
                    Lista = Lista.Skip(skip).Take(data.Take).ToList();

                data.TotalRegistros = TotalRegistros;
                data.Lista = Lista;

                return data;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public bool ConfirmarSeguimiento(int seguimientoId, int UserId)
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                int statusPorConfirmar = (int)Enumeradores.StatusSeguimiento.PorConfirmar;
                int statusConfirmado = (int)Enumeradores.StatusSeguimiento.Confirmado;

                var Seguimiento = (from a in ctx.Seguimientos where
                                   a.EsEliminado == NoEliminado &&
                                   a.StatusSeguimiento == statusPorConfirmar &&
                                   a.SeguimientoId == seguimientoId
                                   select a).FirstOrDefault();

                if (Seguimiento == null)
                    return false;

                Seguimiento.StatusSeguimiento = statusConfirmado;
                Seguimiento.UsuActualiza = UserId;
                Seguimiento.FechaActualiza = DateTime.UtcNow;

                int row = ctx.SaveChanges();

                return row > 0;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool EliminarSeguimiento(int seguimientoId, int UserId)
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                int Eliminado = (int)Enumeradores.EsEliminado.Si;

                var Seguimiento = (from a in ctx.Seguimientos
                                   where
                                    a.EsEliminado == NoEliminado &&
                                    a.SeguimientoId == seguimientoId
                                   select a).FirstOrDefault();

                if (Seguimiento == null)
                    return false;

                Seguimiento.EsEliminado = Eliminado;
                Seguimiento.UsuActualiza = UserId;
                Seguimiento.FechaActualiza = DateTime.UtcNow;

                int row = ctx.SaveChanges();

                return row > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string NotificarSeguimiento(List<int> SeguimientoIdList)
        {
            try
            {
                if (SeguimientoIdList == null)
                    return "Debe Seleccionar al menos a una persona.";

                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                int SiRecibeCorreo = (int)Enumeradores.RecibirCorreoDeArea.Si;
                int grupoEmail = (int)Enumeradores.GrupoParametros.Correo;
                int parametroBody = (int)Enumeradores.Correo.BodyEnvioRegistroUsuario;
                int parametroCorreo = (int)Enumeradores.Correo.CorreoSistema;
                int parametroClave = (int)Enumeradores.Correo.ClaveCorreo;
                int parametroHost = (int)Enumeradores.Correo.HostSMTP;

                var parametros = (from a in ctx.Parametros where a.GrupoId == grupoEmail select a).ToList();

                string CorreoSistema = (from a in parametros where a.ParametroId == parametroCorreo select a.Valor2).FirstOrDefault();
                string ClaveCorreo = (from a in parametros where a.ParametroId == parametroClave select a.Valor2).FirstOrDefault();
                string CorreoHost = (from a in parametros where a.ParametroId == parametroHost select a.Valor2).FirstOrDefault();

                var Data = (from a in ctx.Seguimientos
                            join z in ctx.Colaboradores on a.ColaboradorId equals z.ColaboradorId
                            join b in ctx.Personas on z.PersonaId equals b.PersonaId
                            join c in ctx.Colaboradores on b.PersonaId equals c.PersonaId
                            where 
                            (a.EsEliminado == NoEliminado && b.EsEliminado == NoEliminado && c.EsEliminado == NoEliminado) &&
                            SeguimientoIdList.Contains(a.SeguimientoId)
                            select new { a,b,c }).ToList();

                var ListadoPersonas = Data.GroupBy(x => x.b.PersonaId).Select(x => new {
                                                PersonaID = x.Key,
                                                x.FirstOrDefault().b.Correo,
                                                x.FirstOrDefault().c.Area,
                                                x.FirstOrDefault().c.SedeId
                                                }).ToList();

                var ListadoEmpresas = ListadoPersonas.GroupBy(x => x.SedeId).Select(x => x.FirstOrDefault().SedeId).ToList();

                foreach(var EmpresaId in ListadoEmpresas)
                {
                    var ListadoAreas = ListadoPersonas.Where(x => x.SedeId == EmpresaId).GroupBy(x => x.Area).Select(x => x.FirstOrDefault().Area).ToList();

                    foreach(var Area in ListadoAreas)
                    {
                        var Personas = ListadoPersonas.Where(x => x.Area == Area && x.SedeId == EmpresaId).ToList();

                        string email = (from a in ctx.Colaboradores
                                        join b in ctx.Personas on a.PersonaId equals b.PersonaId
                                        where
                                        a.SedeId == EmpresaId &&
                                        a.Area == Area &&
                                        a.RecibirCorreoDeArea == SiRecibeCorreo &&
                                        a.EsEliminado == NoEliminado &&
                                        b.EsEliminado == NoEliminado
                                        select b.Correo).FirstOrDefault();

                        if (string.IsNullOrWhiteSpace(email))
                            return "No se encuentra responsable del area: " + Area;


                        string body = (from a in parametros where a.ParametroId == parametroBody select a.Campo).FirstOrDefault();

                        string subject = "Registro Exitoso de Usuario";
                        List<string> adresses = new List<string>();
                        adresses.Add(email);

                        Task TASK = new Task(() => Utils.SendMail(body, subject, adresses, CorreoSistema, ClaveCorreo, CorreoHost));
                        TASK.Start();
                    }

                }

                return "";
            }
            catch (Exception e)
            {
                return "Sucedió un error en el servidor... intente más tarde.";
            }
        }

        public bool CambiarSeguimiento(int seguimientoId, string fechaString, int UserId)
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("es-PE");
                DateTime Fecha = DateTime.Parse(fechaString, new CultureInfo("es-PE"), DateTimeStyles.AssumeLocal).ToUniversalTime();

                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                int statusPorConfirmar = (int)Enumeradores.StatusSeguimiento.PorConfirmar;
                int statusConfirmado = (int)Enumeradores.StatusSeguimiento.Confirmado;

                var Seguimiento = (from a in ctx.Seguimientos where
                                   a.EsEliminado == NoEliminado &&
                                   (a.StatusSeguimiento == statusPorConfirmar || a.StatusSeguimiento == statusConfirmado) &&
                                   a.SeguimientoId == seguimientoId
                                   select a).FirstOrDefault();

                if (Seguimiento == null)
                    return false;

                Seguimiento.StatusSeguimiento = statusConfirmado;
                Seguimiento.Fecha = Fecha;
                Seguimiento.UsuActualiza = UserId;
                Seguimiento.FechaActualiza = DateTime.UtcNow;

                int row = ctx.SaveChanges();

                return row > 0;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool AdjuntarCita(int seguimientoId, string archivoBase64, string nombreArchivo, int UserId, string FilePath)
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                int StatusAtendido = (int)Enumeradores.StatusSeguimiento.Atendido;

                var Seguimiento = (from a in ctx.Seguimientos where
                                   a.EsEliminado == NoEsEliminado &&
                                   a.SeguimientoId == seguimientoId
                                   select a).FirstOrDefault();

                if (Seguimiento == null)
                    return false;

                Seguimiento.StatusSeguimiento = StatusAtendido;
                Seguimiento.FechaActualiza = DateTime.UtcNow;
                Seguimiento.UsuActualiza = UserId;

                ctx.SaveChanges();

                string extension = "." + nombreArchivo.Split('.')[nombreArchivo.Split('.').Length - 1];

                nombreArchivo = "\\" + Seguimiento.Fecha.ToString("ddMMyyyy") + Seguimiento.SeguimientoId.ToString() + extension;

                string PathAndFileName = FilePath + nombreArchivo;
                File.WriteAllBytes(PathAndFileName, Convert.FromBase64String(archivoBase64));

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool IngresarControl(int seguimientoId, int valorControl, int UserId)
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                int grupoTipoControl = (int)Enumeradores.GrupoParametros.TipoControl;

                var Seguimiento = (from a in ctx.Seguimientos where a.EsEliminado == NoEsEliminado && a.SeguimientoId == seguimientoId select a).FirstOrDefault();
                if (Seguimiento == null)
                    return false;

                var DiagnosticoSeguimiento = (from a in ctx.DiagnosticosSeguimiento where a.EsEliminado == NoEsEliminado && a.SeguimientoId == Seguimiento.SeguimientoId select a).FirstOrDefault();
                if (DiagnosticoSeguimiento == null)
                    return false;

                var TipoControl = (from a in ctx.Parametros where a.EsEliminado == NoEsEliminado && a.GrupoId == grupoTipoControl && a.ParametroId == DiagnosticoSeguimiento.TipoControlId select a).FirstOrDefault();
                if (TipoControl == null)
                    return false;

                var componenteId = (from a in ctx.Componentes where a.EsEliminado == NoEsEliminado && a.Nombre == TipoControl.Valor1 select a.ComponenteId).FirstOrDefault();

                var ComponenteCampo = (from a in ctx.ComponenteCampos
                                       join b in ctx.ServicioComponentes on a.ServicioComponenteId equals b.ServicioComponenteId
                                       join c in ctx.Servicios on b.ServicioId equals c.ServicioId
                                       where 
                                       a.EsEliminado == NoEsEliminado &&
                                       b.EsEliminado == NoEsEliminado &&
                                       c.EsEliminado == NoEsEliminado &&
                                       c.ServicioId == Seguimiento.ServicioId &&
                                       a.ComponenteId == componenteId &&
                                       c.ColaboradorId == Seguimiento.ColaboradorId
                                       select a).FirstOrDefault();


                ComponenteCampo CC = new ComponenteCampo()
                {
                    ComponenteId = componenteId,
                    EsEliminado = NoEsEliminado,
                    FechaGraba = DateTime.UtcNow,
                    ServicioComponenteId = ComponenteCampo.ServicioComponenteId,
                    UsuGraba = UserId,
                    Valor = valorControl.ToString()
                };

                ctx.ComponenteCampos.Add(CC);
                int rows = ctx.SaveChanges();

                return rows > 0;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public BandejaServicios FiltrarServicios(BandejaServicios data)
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                int grupoTipoDocumento = (int)Enumeradores.GrupoParametros.TipoDocumentos;
                string Nombre = string.IsNullOrWhiteSpace(data.Nombre) ? "" : data.Nombre;
                string NroDocumento = string.IsNullOrWhiteSpace(data.NroDocumento) ? "" : data.NroDocumento;
                int skip = (data.Index - 1) * data.Take;

                var Lista = (from a in ctx.Servicios
                             join b in ctx.Colaboradores on a.ColaboradorId equals b.ColaboradorId
                             join c in ctx.Personas on b.PersonaId equals c.PersonaId
                             join d in ctx.Parametros on new { a = grupoTipoDocumento, b = c.TipoDocumentoId } equals new { a = d.GrupoId, b = d.ParametroId }
                             where 
                             (a.EsEliminado == NoEliminado && b.EsEliminado == NoEliminado && c.EsEliminado == NoEliminado)
                             select new BandejaServiciosDetalle()
                             {
                                 Nombre = c.Nombres + " " + c.ApellidoPaterno + " " + c.ApellidoMaterno,
                                 NroDocumento = c.NroDocumento,
                                 TipoDocumento = d.Valor1,
                                 ServicioId = a.ServicioId
                             }).ToList();

                int TotalRegistros = Lista.Count;

                if (data.Take > 0)
                    Lista = Lista.Skip(skip).Take(data.Take).ToList();

                data.TotalRegistros = TotalRegistros;
                data.Lista = Lista;

                return data;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public List<Dropdownlist> ObtenerListadoTipoSeguimiento()
        {
            try
            {
                int grupoTipoSeguimiento = (int)Enumeradores.GrupoParametros.TipoSeguimiento;
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;

                List<Dropdownlist> result = (from a in ctx.Parametros
                                             where a.GrupoId == grupoTipoSeguimiento && a.EsEliminado == NoEsEliminado
                                             select new Dropdownlist()
                                             {
                                                 Id = a.ParametroId,
                                                 Value = a.Valor1
                                             }).ToList();

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
