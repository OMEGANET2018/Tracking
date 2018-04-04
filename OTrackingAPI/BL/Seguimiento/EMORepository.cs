using DAL;
using BE;
using BE.Comun;
using BE.Administracion;
using BE.Seguimiento;
using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace BL.Seguimiento
{
    public class EMORepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public BandejaEMO ObtenerBandejaEMO(BandejaEMO data, string proveedorClinica = "")
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                int GrupoTipoDocumento = (int)Enumeradores.GrupoParametros.TipoDocumentos;
                int GrupoGenero = (int)Enumeradores.GrupoParametros.Generos;
                int GrupoGradoInstruccion = (int)Enumeradores.GrupoParametros.GradoDeInstruccion;
                int GrupoZona = (int)Enumeradores.GrupoParametros.Zonas;
                int GrupoSiNo = (int)Enumeradores.GrupoParametros.SiNo;
                string nombre = string.IsNullOrWhiteSpace(data.Nombre) ? "" : data.Nombre;
                int skip = (data.Index - 1) * data.Take;

                List<string> listaBusqueda = new List<string>();

                var return_data = (from a in ctx.Personas
                                   join l in ctx.Colaboradores on a.PersonaId equals l.PersonaId
                                   join Empresa in ctx.Empresas on l.SedeId equals Empresa.EmpresaId into GEmp
                                   from k in GEmp.DefaultIfEmpty()
                                   join b in ctx.Parametros on new { a = a.TipoDocumentoId, b = GrupoTipoDocumento } equals new { a = b.ParametroId, b = b.GrupoId }
                                   join c in ctx.Parametros on new { a = a.GeneroId.HasValue ? a.GeneroId.Value : 0, b = GrupoGenero } equals new { a = c.ParametroId, b = c.GrupoId } into grupoC
                                   from d in grupoC.DefaultIfEmpty()
                                   join e in ctx.Parametros on new { a = l.GradoInstruccionId.HasValue ? l.GradoInstruccionId.Value : 0, b = GrupoGradoInstruccion } equals new { a = e.ParametroId, b = e.GrupoId } into grupoE
                                   from f in grupoE.DefaultIfEmpty()
                                   join g in ctx.Parametros on new { a = l.ZonaId.HasValue ? l.ZonaId.Value : 0, b = GrupoZona } equals new { a = g.ParametroId, b = g.GrupoId } into grupoG
                                   from h in grupoG.DefaultIfEmpty()
                                   join i in ctx.Parametros on new { a = l.DiscapacitadoId.HasValue ? l.DiscapacitadoId.Value : 0, b = GrupoSiNo } equals new { a = i.ParametroId, b = i.GrupoId } into grupoI
                                   from j in grupoI.DefaultIfEmpty()
                                   where a.EsEliminado == NoEsEliminado &&
                                        b.EsEliminado == NoEsEliminado &&
                                        k.EsEliminado == NoEsEliminado &&
                                        (a.Nombres + " " + a.ApellidoPaterno + " " + a.ApellidoMaterno).Contains(nombre)
                                   select new BandejaEMODetalle()
                                   {
                                       TipoDocumentoId = a.TipoDocumentoId,
                                       TipoDocumento = b.Valor1,
                                       NroDocumento = a.NroDocumento,
                                       Nombres = a.Nombres,
                                       ApellidoPaterno = a.ApellidoPaterno,
                                       ApellidoMaterno = a.ApellidoMaterno,
                                       FechaNacimiento = a.FechaNacimiento,
                                       GeneroId = a.GeneroId,
                                       Genero = d.Valor1,
                                       GradoDeInstruccionId = l.GradoInstruccionId,
                                       GradoDeInstruccion = f.Valor1,
                                       PuestoLaboral = l.PuestoLaboral,
                                       Area = l.Area,
                                       ZonaId = l.ZonaId,
                                       Zona = h.Valor1,
                                       LugarDeTrabajo = l.LugarDeTrabajo,
                                       DiscapacitadoId = l.DiscapacitadoId,
                                       Discapacitado = j.Valor1,
                                       ProveedorClinica = proveedorClinica,
                                       SedeRUC = k.Ruc
                                   }).ToList();

                if (data.Lista != null)
                {
                    listaBusqueda = data.Lista.Select(x => x.NroDocumento).ToList();
                    return_data = return_data.Where(x => listaBusqueda.Contains(x.NroDocumento)).ToList();
                }

                foreach (var A in return_data)
                {
                    A.FechaNacimiento = A.FechaNacimiento.HasValue ? (DateTime?)DateTime.Parse(A.FechaNacimiento.Value.ToString("yyyy-MM-ddThh:mm:ss"), System.Globalization.CultureInfo.CurrentCulture,System.Globalization.DateTimeStyles.AssumeUniversal) : null;
                    A.Edad = A.FechaNacimiento.HasValue ? (int?)DateTime.UtcNow.AddTicks(-A.FechaNacimiento.Value.Ticks).Year : null;
                }

                int TotalRegistros = return_data.Count;

                if (data.Take > 0)
                    return_data = return_data.Skip(skip).Take(data.Take).ToList();

                data.TotalRegistros = TotalRegistros;

                data.Lista = return_data;

                return data;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public MemoryStream CrearEMO(FileStream TemplateFile,BandejaEMO EMO)
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-PE");
                IWorkbook TemplateBook = new XSSFWorkbook(TemplateFile);
                ISheet TemplateSheet = TemplateBook.GetSheet(TemplateBook.GetSheetName(0));

                int index = 6;
                int contador = 0;

                var clinica = (from a in ctx.Empresas where a.EmpresaId == EMO.EmpresaId select a).FirstOrDefault();

                IRow TemplateRow = TemplateSheet.GetRow(index);
                ICellStyle EstiloObscuro = TemplateRow.GetCell(0).CellStyle;
                ICellStyle EstiloClaro = TemplateRow.GetCell(1).CellStyle;

                TemplateSheet.RemoveRow(TemplateRow);

                var datalist = ObtenerBandejaEMO(EMO,clinica == null ? "" : clinica.RazonSocial);

                foreach (var data in datalist.Lista)
                {
                    TemplateRow = TemplateSheet.CreateRow(index);

                    ICell TemplateCell = TemplateRow.CreateCell(0);
                    TemplateCell.SetCellValue(contador);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(1);
                    TemplateCell.SetCellValue(data.TipoDocumento);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(2);
                    TemplateCell.SetCellValue(data.TipoDocumentoId);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(3);
                    TemplateCell.SetCellValue(data.NroDocumento);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(4);
                    TemplateCell.SetCellValue(data.Nombres);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(5);
                    TemplateCell.SetCellValue(data.ApellidoPaterno);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(6);
                    TemplateCell.SetCellValue(data.ApellidoMaterno);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(7);
                    TemplateCell.SetCellValue(data.FechaNacimiento.HasValue ? data.FechaNacimiento.Value.ToString("dd/MM/yyyy") : "");
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(8);
                    TemplateCell.SetCellValue(data.Edad.HasValue ? data.Edad.Value.ToString() : null);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(9);
                    TemplateCell.SetCellValue(data.Genero);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(10);
                    TemplateCell.SetCellValue(data.GeneroId.HasValue ? data.GeneroId.Value.ToString() : "");
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(11);
                    TemplateCell.SetCellValue(data.GradoDeInstruccion);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(12);
                    TemplateCell.SetCellValue(data.GradoDeInstruccionId.HasValue ? data.GradoDeInstruccionId.Value.ToString() : "");
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(13);
                    TemplateCell.SetCellValue(data.PuestoLaboral);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(14);
                    TemplateCell.SetCellValue(data.Area);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(15);
                    TemplateCell.SetCellValue(data.Zona);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(16);
                    TemplateCell.SetCellValue(data.ZonaId.HasValue ? data.ZonaId.Value.ToString() : "");
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(17);
                    TemplateCell.SetCellValue(data.LugarDeTrabajo);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(18);
                    TemplateCell.SetCellValue(data.Discapacitado);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(19);
                    TemplateCell.SetCellValue(data.DiscapacitadoId.HasValue ? data.DiscapacitadoId.Value.ToString() : "");
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(20);
                    TemplateCell.SetCellValue(data.ProveedorClinica);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(21);
                    TemplateCell.SetCellValue(data.SedeRUC);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(35);
                    TemplateCell.CellStyle = EstiloObscuro;

                    TemplateCell = TemplateRow.CreateCell(40);
                    TemplateCell.CellStyle = EstiloObscuro;

                    index++;
                    contador++;
                }

                MemoryStream ms = new MemoryStream();
                using (MemoryStream tempStream = new MemoryStream())
                {
                    TemplateBook.Write(tempStream);
                    var byteArray = tempStream.ToArray();
                    ms.Write(byteArray, 0, byteArray.Length);
                }

                if (EMO.EnviarCorreo)
                {
                    int grupoEmail = (int)Enumeradores.GrupoParametros.Correo;
                    int parametroBody = (int)Enumeradores.Correo.BodyEnvioEMO;
                    int parametroCorreo = (int)Enumeradores.Correo.CorreoSistema;
                    int parametroClave = (int)Enumeradores.Correo.ClaveCorreo;
                    int parametroHost = (int)Enumeradores.Correo.HostSMTP;

                    var parametros = (from a in ctx.Parametros where a.GrupoId == grupoEmail select a).ToList();

                    string CorreoSistema = (from a in parametros where a.ParametroId == parametroCorreo select a.Valor2).FirstOrDefault();
                    string ClaveCorreo = (from a in parametros where a.ParametroId == parametroClave select a.Valor2).FirstOrDefault();
                    string CorreoHost = (from a in parametros where a.ParametroId == parametroHost select a.Valor2).FirstOrDefault();

                    string body = (from a in parametros where a.ParametroId == parametroBody select a.Campo).FirstOrDefault();

                    string subject = "Envio de EMO";
                    List<string> adresses = new List<string>();
                    adresses.Add(clinica.Email);

                    Dictionary<string,MemoryStream> Attachments = new Dictionary<string,MemoryStream>();

                    Attachments.Add("EMO", ms);

                    Task TASK = new Task(() => Utils.SendMail(body, subject, adresses, CorreoSistema, ClaveCorreo, CorreoHost, Attachments));
                    TASK.Start();
                }

                if (EMO.DescargarArchivo)
                    return ms;

                return new MemoryStream();
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public object EnviarEMO(MemoryStream stream)
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("es-PE");
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                int RegistrosInsertados = 0;
                int RegistrosErrados = 0;

                IWorkbook book = new XSSFWorkbook(stream);
                ISheet Sheet = book.GetSheet("EMO");
                int index = 6;

                var empresas = (from a in ctx.Empresas where a.EsEliminado == NoEsEliminado select a).ToList();
                var componentesPadres = (from a in ctx.Componentes where a.EsEliminado == NoEsEliminado && a.PadreId == 0 select a).ToList();
                var componentesHijos = (from a in ctx.Componentes where a.EsEliminado == NoEsEliminado && a.PadreId != 0 select a).ToList();
                var PlanesDeVida = (from a in ctx.PlanesDeVida where a.EsEliminado == NoEsEliminado select a).ToList();
                var DiagnosticosPlanesDeVida = (from a in ctx.DiagnosticoPlanesDeVida where a.EsEliminado == NoEsEliminado select a).ToList();
                var CitaPlanesDeVida = (from a in ctx.CitaPlanesDeVida where a.EsEliminado == NoEsEliminado select a).ToList();

                bool finArchivo = false;
                while (!finArchivo)
                {
                    try
                    {
                        index++;
                        IRow Row = Sheet.GetRow(index);
                        List<string> ListaCIE10 = new List<string>();

                        if (Row != null)
                        {
                            string NroDocumento = Row.GetCell(3)?.ToString();
                            DateTime? fechaexamen = Row.GetCell(22) == null ? null : (DateTime?)DateTime.Parse(Row.GetCell(22).ToString(),CultureInfo.CurrentCulture,DateTimeStyles.AssumeLocal).ToUniversalTime();

                            var persona = (from a in ctx.Personas where a.NroDocumento == NroDocumento && a.EsEliminado == NoEsEliminado select a).FirstOrDefault();

                            if (persona == null)
                                throw new Exception();

                            var colaborador = (from a in ctx.Colaboradores where a.PersonaId == persona.PersonaId && a.EsEliminado == NoEsEliminado select a).FirstOrDefault();

                            if (colaborador == null)
                                throw new Exception();

                            #region INFORMACION DEL SERVICIO
                            Servicio servicio = new Servicio()
                            {
                                AptitudId = Row.GetCell(26) == null ? null : (int?)Row.GetCell(26).NumericCellValue,
                                ColaboradorId = colaborador.ColaboradorId,
                                EsEliminado = NoEsEliminado,
                                FechaExamen = fechaexamen,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                TipoExamenId = (int)Row.GetCell(24).NumericCellValue,
                                ProoveedorId = empresas.Where(x => x.RazonSocial.ToUpper() == Row.GetCell(20).ToString().ToUpper()).Select(x => x.EmpresaId).FirstOrDefault()
                            };

                            ctx.Servicios.Add(servicio);
                            ctx.SaveChanges();

                            if (servicio.ServicioId == 0)
                                throw new Exception();
                            #endregion

                            #region Hábitos Noscivos
                            ServicioComponente SC = new ServicioComponente()
                            {
                                ComponenteId = componentesPadres.Where(x => x.Nombre == "Hábitos Noscivos").Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                ServicioId = servicio.ServicioId
                            };
                            ctx.ServicioComponentes.Add(SC);
                            ctx.SaveChanges();

                            ComponenteCampo CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Fumar" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(28).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Licor" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(30).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Drogas" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(32).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();
                            #endregion

                            #region Trieaje
                            SC = new ServicioComponente()
                            {
                                ComponenteId = componentesPadres.Where(x => x.Nombre == "Triaje").Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                ServicioId = servicio.ServicioId
                            };
                            ctx.ServicioComponentes.Add(SC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Peso" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(33).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Talla" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(34).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Cintura" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(38).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Cadera" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(39).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Sistolica" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(41).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Diastolica" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(44).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "FC" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(47).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "FR" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(48).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            ComponenteDiagnostico CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(36).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(37)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(42).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(43)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(45).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(46)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);
                            #endregion

                            #region Oftalmo
                            SC = new ServicioComponente()
                            {
                                ComponenteId = componentesPadres.Where(x => x.Nombre == "Oftalmo").Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                ServicioId = servicio.ServicioId
                            };
                            ctx.ServicioComponentes.Add(SC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Cerca OD" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(49).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Cerca OI" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(50).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Lejos OD" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(51).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Lejos OI" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(52).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Cerca OD" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(53).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Cerca OI" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(54).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Lejos OD" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(55).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Lejos OI" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(56).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Discro" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(61).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(57).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(58)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(59).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(60)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(62).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(63)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);
                            #endregion

                            #region Audiometria
                            SC = new ServicioComponente()
                            {
                                ComponenteId = componentesPadres.Where(x => x.Nombre == "Audiometria").Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                ServicioId = servicio.ServicioId
                            };
                            ctx.ServicioComponentes.Add(SC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Otoscopia OD" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(64).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Otoscopia OI" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(65).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Der_125" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(65).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Der_250" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(66).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Der_500" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(67).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Der_750" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(68).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Der_1000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(69).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Der_1500" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(70).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Der_2000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(71).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Der_3000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(72).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Der_4000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(73).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Der_6000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(74).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Der_8000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(75).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Izq_125" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(76).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Izq_250" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(77).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Izq_500" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(78).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Izq_750" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(79).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Izq_1000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(80).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Izq_1500" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(81).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Izq_2000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(82).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Izq_3000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(83).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Izq_4000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(84).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Izq_6000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(85).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Oido_Izq_8000" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(86).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(87).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(88)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(89).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(90)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);
                            #endregion

                            #region Laboratorio
                            SC = new ServicioComponente()
                            {
                                ComponenteId = componentesPadres.Where(x => x.Nombre == "Laboratorio").Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                ServicioId = servicio.ServicioId
                            };
                            ctx.ServicioComponentes.Add(SC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Grupo Sanguineo" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(92).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Factor RH" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(94).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Hemoglobina" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(95).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Colesterol" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(98).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Trigliceridos" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(101).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "Glucosa" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(104).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(96).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(97)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(99).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(100)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(102).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(103)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(105).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(106)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);
                            #endregion

                            #region Espiro
                            SC = new ServicioComponente()
                            {
                                ComponenteId = componentesPadres.Where(x => x.Nombre == "Espiro").Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                ServicioId = servicio.ServicioId
                            };
                            ctx.ServicioComponentes.Add(SC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "FEV1 Teorico" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(107).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CC = new ComponenteCampo()
                            {
                                ComponenteId = componentesHijos.Where(x => x.Nombre == "FVC Teorico" && x.PadreId == SC.ComponenteId).Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0,
                                Valor = Row.GetCell(108).ToString()
                            };
                            ctx.ComponenteCampos.Add(CC);
                            ctx.SaveChanges();

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(109).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(110)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);
                            #endregion

                            #region Medicina
                            SC = new ServicioComponente()
                            {
                                ComponenteId = componentesPadres.Where(x => x.Nombre == "Medicina").Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                ServicioId = servicio.ServicioId
                            };
                            ctx.ServicioComponentes.Add(SC);
                            ctx.SaveChanges();

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(111).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(112)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(113).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(114)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);
                            #endregion

                            #region Odonto
                            SC = new ServicioComponente()
                            {
                                ComponenteId = componentesPadres.Where(x => x.Nombre == "Odonto").Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                ServicioId = servicio.ServicioId
                            };
                            ctx.ServicioComponentes.Add(SC);
                            ctx.SaveChanges();

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(115).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(116)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);
                            #endregion

                            #region EKG
                            SC = new ServicioComponente()
                            {
                                ComponenteId = componentesPadres.Where(x => x.Nombre == "EKG").Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                ServicioId = servicio.ServicioId
                            };
                            ctx.ServicioComponentes.Add(SC);
                            ctx.SaveChanges();

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(117).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(118)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);
                            #endregion

                            #region Rayos X
                            SC = new ServicioComponente()
                            {
                                ComponenteId = componentesPadres.Where(x => x.Nombre == "Rayos X").Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                ServicioId = servicio.ServicioId
                            };
                            ctx.ServicioComponentes.Add(SC);
                            ctx.SaveChanges();

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(119).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(120)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);
                            #endregion

                            #region Psicologia
                            SC = new ServicioComponente()
                            {
                                ComponenteId = componentesPadres.Where(x => x.Nombre == "Psicologia").Select(x => x.ComponenteId).FirstOrDefault(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                UsuGraba = 0,
                                ServicioId = servicio.ServicioId
                            };
                            ctx.ServicioComponentes.Add(SC);
                            ctx.SaveChanges();

                            CD = new ComponenteDiagnostico()
                            {
                                CIE10Id = Row.GetCell(121).ToString(),
                                EsEliminado = NoEsEliminado,
                                FechaGraba = DateTime.UtcNow,
                                Observacion = Row.GetCell(122)?.ToString(),
                                ServicioComponenteId = SC.ServicioComponenteId,
                                UsuGraba = 0
                            };
                            ctx.ComponenteDiagnosticos.Add(CD);
                            ctx.SaveChanges();
                            ListaCIE10.Add(CD.CIE10Id);
                            #endregion

                            #region Planes De Vida
                            foreach(var P in PlanesDeVida)
                            {
                                bool ingresarEnPlan = true;
                                foreach(var D in DiagnosticosPlanesDeVida.Where(x => x.PlanesDeVidaId == P.PlanesDeVidaId).ToList())
                                {
                                    if (!ListaCIE10.Contains(D.CIE10Id))
                                        ingresarEnPlan = false;
                                }

                                if (ingresarEnPlan)
                                {
                                    List<string> TempCIE10 = new List<string>();
                                    foreach (var D in DiagnosticosPlanesDeVida.Where(x => x.PlanesDeVidaId == P.PlanesDeVidaId).ToList())
                                    {
                                        DateTime FechaParaElControl = DateTime.UtcNow;
                                        switch (D.ControlNumeroTipoId)
                                        {
                                            case (int)Enumeradores.TipoTiempo.Dia:
                                                {
                                                    FechaParaElControl.AddDays(D.ControlNumero.Value);
                                                    break;
                                                }
                                            case (int)Enumeradores.TipoTiempo.Semana:
                                                {
                                                    FechaParaElControl.AddDays(D.ControlNumero.Value * 7);
                                                    break;
                                                }
                                            case (int)Enumeradores.TipoTiempo.Mes:
                                                {
                                                    FechaParaElControl.AddMonths(D.ControlNumero.Value);
                                                    break;
                                                }
                                            case (int)Enumeradores.TipoTiempo.Año:
                                                {
                                                    FechaParaElControl.AddYears(D.ControlNumero.Value);
                                                    break;
                                                }
                                        }
                                        BE.Seguimiento.Seguimiento seguimiento = new BE.Seguimiento.Seguimiento()
                                        {
                                            EsEliminado = NoEsEliminado,
                                            Fecha = FechaParaElControl,
                                            FechaGraba = DateTime.UtcNow,
                                            UsuGraba = 0,
                                            ColaboradorId = colaborador.ColaboradorId,
                                            StatusSeguimiento = (int)Enumeradores.StatusSeguimiento.PorConfirmar,
                                            TipoSeguimiento = (int)Enumeradores.TipoSeguimiento.Control,
                                            ServicioId = servicio.ServicioId
                                        };
                                        ctx.Seguimientos.Add(seguimiento);
                                        ctx.SaveChanges();

                                        DiagnosticoSeguimiento DxSeguimiento = new DiagnosticoSeguimiento()
                                        {
                                            CIE10Id = D.CIE10Id,
                                            EsEliminado = NoEsEliminado,
                                            FechaGraba = DateTime.UtcNow,
                                            UsuGraba = 0,
                                            SeguimientoId = seguimiento.SeguimientoId,
                                            TipoControlId = D.TipoControlId
                                        };
                                        ctx.DiagnosticosSeguimiento.Add(DxSeguimiento);
                                        ctx.SaveChanges();
                                        TempCIE10.Add(D.CIE10Id);
                                    }

                                    foreach(var C in CitaPlanesDeVida.Where(x => x.PlanesDeVidaId == P.PlanesDeVidaId).ToList())
                                    {
                                        DateTime FechaParaElControl = DateTime.UtcNow;
                                        switch (C.NumeroTipoId)
                                        {
                                            case (int)Enumeradores.TipoTiempo.Dia:
                                                {
                                                    FechaParaElControl.AddDays(C.Numero.Value);
                                                    break;
                                                }
                                            case (int)Enumeradores.TipoTiempo.Semana:
                                                {
                                                    FechaParaElControl.AddDays(C.Numero.Value * 7);
                                                    break;
                                                }
                                            case (int)Enumeradores.TipoTiempo.Mes:
                                                {
                                                    FechaParaElControl.AddMonths(C.Numero.Value);
                                                    break;
                                                }
                                            case (int)Enumeradores.TipoTiempo.Año:
                                                {
                                                    FechaParaElControl.AddYears(C.Numero.Value);
                                                    break;
                                                }
                                        }
                                        BE.Seguimiento.Seguimiento seguimiento = new BE.Seguimiento.Seguimiento()
                                        {
                                            EsEliminado = NoEsEliminado,
                                            Fecha = FechaParaElControl,
                                            FechaGraba = DateTime.UtcNow,
                                            UsuGraba = 0,
                                            ColaboradorId = colaborador.ColaboradorId,
                                            StatusSeguimiento = (int)Enumeradores.StatusSeguimiento.PorConfirmar,
                                            TipoSeguimiento = (int)Enumeradores.TipoSeguimiento.Consulta,
                                            ServicioId = servicio.ServicioId
                                        };
                                        ctx.Seguimientos.Add(seguimiento);
                                        ctx.SaveChanges();

                                        foreach(var CIE10 in TempCIE10)
                                        {
                                            DiagnosticoSeguimiento DxSeguimiento = new DiagnosticoSeguimiento()
                                            {
                                                CIE10Id = CIE10,
                                                EsEliminado = NoEsEliminado,
                                                FechaGraba = DateTime.UtcNow,
                                                UsuGraba = 0,
                                                SeguimientoId = seguimiento.SeguimientoId
                                            };
                                            ctx.DiagnosticosSeguimiento.Add(DxSeguimiento);
                                            ctx.SaveChanges();
                                        }
                                    }
                                }
                            }

                            #endregion

                            RegistrosInsertados++;
                        }
                        else
                        {
                            finArchivo = true;
                        }
                    }
                    catch (Exception e)
                    {
                        RegistrosErrados++;
                    }
                }
                return new { RegistrosInsertados, RegistrosErrados };
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
