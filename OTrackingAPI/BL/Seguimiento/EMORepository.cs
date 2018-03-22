using DAL;
using BE.Comun;
using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Linq;
using System.Threading.Tasks;
using BE;

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
                    A.Edad = A.FechaNacimiento.HasValue ? (int?)DateTime.Now.AddTicks(-A.FechaNacimiento.Value.Ticks).Year : null;
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
    }
}
