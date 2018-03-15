using DAL;
using BE;
using BE.Persona;
using BE.Comun;
using System;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;

namespace BL
{
   
    public class PersonaRepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public object InsertarPersonaExcel(MemoryStream stream)
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                int RegistrosInsertados = 0;
                int RegistrosErrados = 0;

                IWorkbook book = new XSSFWorkbook(stream);
                ISheet Sheet = book.GetSheet("Persona");
                int index = 3;


                bool finArchivo = false;
                while (!finArchivo)
                {
                    try
                    {
                        index++;
                        IRow Row = Sheet.GetRow(index);

                        if(Row != null)
                        {
                            int? dia = Row.GetCell(11) != null ? (int?)Row.GetCell(11).NumericCellValue : null;
                            int? mes = Row.GetCell(12) != null ? (int?)Row.GetCell(12).NumericCellValue : null;
                            int? año = Row.GetCell(13) != null ? (int?)Row.GetCell(13).NumericCellValue : null;
                            DateTime? FechaNacimiento = null;
                            if (dia.HasValue && mes.HasValue && año.HasValue)
                            {
                                if (DateTime.TryParse(string.Format("{0}-{1}-{2}T00:00:0000000000+00:00", año.Value, mes.Value, dia.Value),CultureInfo.InvariantCulture,DateTimeStyles.AssumeUniversal, out DateTime F))
                                    FechaNacimiento = F;
                            }

                            dia = Row.GetCell(20) != null ? (int?)Row.GetCell(20).NumericCellValue : null;
                            mes = Row.GetCell(21) != null ? (int?)Row.GetCell(21).NumericCellValue : null;
                            año = Row.GetCell(22) != null ? (int?)Row.GetCell(22).NumericCellValue : null;
                            DateTime? FechaIngreso = null;
                            if (dia.HasValue && mes.HasValue && año.HasValue)
                            {
                                if (DateTime.TryParse(string.Format("{0}-{1}-{2}T00:00:0000000000+00:00", año.Value, mes.Value, dia.Value), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime F))
                                    FechaIngreso = F;
                            }

                            Persona p = new Persona()
                            {
                                Nombres = Row.GetCell(1) != null ? Row.GetCell(1).ToString() : "",
                                ApellidoPaterno = Row.GetCell(2) != null ? Row.GetCell(2).ToString() : "",
                                ApellidoMaterno = Row.GetCell(3) != null ? Row.GetCell(3).ToString() : "",
                                TipoDocumentoId = Row.GetCell(5) != null ? (int)Row.GetCell(5).NumericCellValue : 0,
                                NroDocumento = Row.GetCell(6) != null ? Row.GetCell(6).ToString() : "",
                                GeneroId = Row.GetCell(8) != null ? (int?)Row.GetCell(8).NumericCellValue : null,
                                FechaNacimiento = FechaNacimiento,
                                LugarNacimiento = Row.GetCell(14) != null ? Row.GetCell(14).ToString() : "",
                                Telefono = Row.GetCell(15) != null ? Row.GetCell(15).ToString() : "",
                                Correo = Row.GetCell(16) != null ? Row.GetCell(16).ToString() : "",
                                EstadoCivilId = Row.GetCell(18) != null ? (int?)Row.GetCell(18).NumericCellValue : null,
                                FechaGraba = DateTime.UtcNow,
                                EsEliminado = NoEsEliminado
                            };

                            Persona Persona = (from a in ctx.Personas where a.TipoDocumentoId == p.TipoDocumentoId && a.NroDocumento == p.NroDocumento && a.EsEliminado == NoEsEliminado select a).FirstOrDefault();

                            if(Persona == null)
                            {
                                Persona = p;

                                ctx.Personas.Add(Persona);
                                ctx.SaveChanges();
                            }

                            Colaborador c = new Colaborador()
                            {
                                GradoInstruccionId = Row.GetCell(10) != null ? (int?)Row.GetCell(10).NumericCellValue : null,
                                Direccion = Row.GetCell(19) != null ? Row.GetCell(19).ToString() : "",
                                FechaIngreso = FechaIngreso,
                                PuestoLaboral = Row.GetCell(23) != null ? Row.GetCell(23).ToString() : "",
                                Area = Row.GetCell(24) != null ? Row.GetCell(24).ToString() : "",
                                SedeId = Row.GetCell(25) != null ? (int?)Row.GetCell(25).NumericCellValue : null,
                                DiscapacitadoId = Row.GetCell(27) != null ? (int?)Row.GetCell(27).NumericCellValue : null,
                                FechaGraba = DateTime.UtcNow,
                                EsEliminado = NoEsEliminado,
                                PersonaId = Persona.PersonaId
                            };

                            Colaborador Colaborador = (from a in ctx.Colaboradores where a.PersonaId == Persona.PersonaId && a.EsEliminado == NoEsEliminado select a).FirstOrDefault();

                            if(Colaborador == null)
                            {
                                Colaborador = c;

                                ctx.Colaboradores.Add(Colaborador);
                                ctx.SaveChanges();
                            }

                            RegistrosInsertados++;
                        }
                        else
                        {
                            finArchivo = true;
                        }
                    }
                    catch(Exception e)
                    {
                        RegistrosErrados++;
                    }
                }
                return new { RegistrosInsertados, RegistrosErrados};
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
