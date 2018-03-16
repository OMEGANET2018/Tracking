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

namespace BL.Administracion
{
    public class EMORepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public BandejaEMO ObtenerBandejaEMO(BandejaEMO data)
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                int GrupoTipoDocumento = (int)Enumeradores.GrupoParametros.TipoDocumentos;
                int GrupoGenero = (int)Enumeradores.GrupoParametros.Generos;
                int GrupoGradoInstruccion = (int)Enumeradores.GrupoParametros.GradoDeInstruccion;
                int GrupoZona = (int)Enumeradores.GrupoParametros.Zonas;
                int GrupoSiNo = (int)Enumeradores.GrupoParametros.SiNo;

                var Parametros = (from a in ctx.Parametros where a.EsEliminado == NoEsEliminado select a).ToList();

                var return_data = (from a in ctx.Personas
                                   join b in ctx.Colaboradores on a.PersonaId equals b.PersonaId
                                   where a.EsEliminado == NoEsEliminado
                                   select new BandejaEMODetalle()
                                   {
                                       TipoDocumentoId = a.TipoDocumentoId,
                                       NroDocumento = a.NroDocumento,
                                       Nombres = a.Nombres,
                                       ApellidoPaterno = a.ApellidoPaterno,
                                       ApellidoMaterno = a.ApellidoMaterno,
                                       FechaNacimiento = a.FechaNacimiento,
                                       GeneroId = a.GeneroId,
                                       GradoDeInstruccionId = b.GradoInstruccionId,
                                       PuestoLaboral = b.PuestoLaboral,
                                       Area = b.Area,
                                       ZonaId = b.ZonaId,
                                       LugarDeTrabajo = b.LugarDeTrabajo,
                                       DiscapacitadoId = b.DiscapacitadoId,
                                       ProveedorClinica = "",
                                       SedeRUC = ""
                                   }).ToList();

                data.Lista = (from a in return_data
                              join b in Parametros on new { a = a.TipoDocumentoId, b = GrupoTipoDocumento } equals new { a = b.ParametroId, b = b.GrupoId }
                              join c in Parametros on new { a = a.GeneroId.HasValue ? a.GeneroId.Value : 0, b = GrupoGenero } equals new { a = c.ParametroId, b = c.GrupoId } into grupoC
                              from d in grupoC.DefaultIfEmpty()
                              join e in Parametros on new { a = a.GradoDeInstruccionId.HasValue ? a.GradoDeInstruccionId.Value : 0, b = GrupoGradoInstruccion } equals new { a = e.ParametroId, b = e.GrupoId } into grupoE
                              from f in grupoE.DefaultIfEmpty()
                              join g in Parametros on new { a = a.ZonaId.HasValue ? a.ZonaId.Value : 0, b = GrupoZona } equals new { a = g.ParametroId, b = g.GrupoId } into grupoG
                              from h in grupoG.DefaultIfEmpty()
                              join i in Parametros on new { a = a.DiscapacitadoId.HasValue ? a.DiscapacitadoId.Value : 0, b = GrupoSiNo } equals new { a = i.ParametroId, b = i.GrupoId } into grupoI
                              from j in grupoI.DefaultIfEmpty()
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
                                  GradoDeInstruccionId = a.GradoDeInstruccionId,
                                  GradoDeInstruccion = f.Valor1,
                                  PuestoLaboral = a.PuestoLaboral,
                                  Area = a.Area,
                                  ZonaId = a.ZonaId,
                                  Zona = h.Valor1,
                                  LugarDeTrabajo = a.LugarDeTrabajo,
                                  DiscapacitadoId = a.DiscapacitadoId,
                                  Discapacitado = j.Valor1,
                                  ProveedorClinica = a.ProveedorClinica,
                                  SedeRUC = a.SedeRUC,
                                  Edad = a.FechaNacimiento.HasValue ? (int?)DateTime.UtcNow.AddTicks(- a.FechaNacimiento.Value.Ticks).Year : null,
                                  ICC = 0,
                                  IMC = 0
                              }).ToList();

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
                IWorkbook TemplateBook = new XSSFWorkbook(TemplateFile);
                ISheet TemplateSheet = TemplateBook.GetSheet(TemplateBook.GetSheetName(0));

                int index = 7;
                int contador = 0;

                IRow TemplateRow = TemplateSheet.CreateRow(index);
                ICellStyle EstiloObscuro = TemplateRow.GetCell(0).CellStyle;
                ICellStyle EstiloClaro = TemplateRow.GetCell(1).CellStyle;

                TemplateSheet.RemoveRow(TemplateRow);

                var datalist = ObtenerBandejaEMO(EMO);

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

                    ///////////////////////////////////////////////////

                    TemplateCell = TemplateRow.CreateCell(35);
                    TemplateCell.SetCellValue(data.IMC.HasValue ? data.IMC.Value.ToString("N2") : "");
                    TemplateCell.CellStyle = EstiloObscuro;

                    ////////////////////////////////////////////////////

                    TemplateCell = TemplateRow.CreateCell(40);
                    TemplateCell.SetCellValue(data.ICC.HasValue ? data.ICC.Value.ToString("N2") : "");
                    TemplateCell.CellStyle = EstiloObscuro;

                    ////////////////////////////////////////////////////////////

                    index++;
                    contador++;
                }

                for (int i = 0; i <= 116; i++)
                {
                    TemplateSheet.AutoSizeColumn(i);
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
                    adresses.Add(EMO.Correos);

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
