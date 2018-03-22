using DAL;
using BE.Persona;
using BE.Comun;
using System;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Collections.Generic;
using BE.Acceso;
using BE;
using System.Threading.Tasks;

namespace BL.Personas
{
   
    public class PersonaRepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public object InsertarPersonaExcel(MemoryStream stream)
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("es-PE");
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                int RegistrosInsertados = 0;
                int RegistrosErrados = 0;

                IWorkbook book = new XSSFWorkbook(stream);
                ISheet Sheet = book.GetSheet("Persona");
                int index = 3;

                var empresas = (from a in ctx.Empresas where a.EsEliminado == NoEsEliminado select a).ToList();


                bool finArchivo = false;
                while (!finArchivo)
                {
                    try
                    {   
                        index++;
                        IRow Row = Sheet.GetRow(index);

                        if(Row != null)
                        {
                            int? dia = Row.GetCell(11) != null ? (int?)int.Parse(Row.GetCell(11).ToString()) : null;
                            int? mes = Row.GetCell(12) != null ? (int?)int.Parse(Row.GetCell(12).ToString()) : null;
                            int? año = Row.GetCell(13) != null ? (int?)int.Parse(Row.GetCell(13).ToString()) : null;
                            DateTime? FechaNacimiento = null;
                            if (dia.HasValue && mes.HasValue && año.HasValue)
                            {
                                if (DateTime.TryParse(string.Format("{0}-{1}-{2}T00:00:00", año.Value, mes.Value, dia.Value), CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime F))
                                    FechaNacimiento = F.ToUniversalTime();
                            }

                            dia = Row.GetCell(20) != null ? (int?)int.Parse(Row.GetCell(20).ToString()) : null;
                            mes = Row.GetCell(21) != null ? (int?)int.Parse(Row.GetCell(21).ToString()) : null;
                            año = Row.GetCell(22) != null ? (int?)int.Parse(Row.GetCell(22).ToString()) : null;
                            DateTime? FechaIngreso = null;
                            if (dia.HasValue && mes.HasValue && año.HasValue)
                            {
                                if (DateTime.TryParse(string.Format("{0}-{1}-{2}T00:00:00", año.Value, mes.Value, dia.Value), CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime F))
                                    FechaIngreso = F.ToUniversalTime();
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

                            int? empresaId = null;

                            if (Row.GetCell(25) != null)
                                empresaId = empresas.Where(x => x.Ruc == Row.GetCell(25).ToString())?.Select(x => x.EmpresaId).FirstOrDefault();

                            Colaborador c = new Colaborador()
                            {
                                GradoInstruccionId = Row.GetCell(10) != null ? (int?)Row.GetCell(10).NumericCellValue : null,
                                Direccion = Row.GetCell(19) != null ? Row.GetCell(19).ToString() : "",
                                FechaIngreso = FechaIngreso,
                                PuestoLaboral = Row.GetCell(23) != null ? Row.GetCell(23).ToString() : "",
                                Area = Row.GetCell(24) != null ? Row.GetCell(24).ToString() : "",
                                SedeId = empresaId,
                                DiscapacitadoId = Row.GetCell(27) != null ? (int?)Row.GetCell(27).NumericCellValue : null,
                                FechaGraba = DateTime.UtcNow,
                                EsEliminado = NoEsEliminado,
                                PersonaId = Persona.PersonaId,
                                ZonaId = Row.GetCell(29) != null ? (int?)Row.GetCell(29).NumericCellValue : null,
                                LugarDeTrabajo = Row.GetCell(30) != null ? Row.GetCell(30).ToString() : ""
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

        public List<Dropdownlist> GetGeneros()
        {
            int grupo = (int)Enumeradores.GrupoParametros.Generos;
            int NoEliminado = (int)Enumeradores.EsEliminado.No;
            List<Dropdownlist> result = (from a in ctx.Parametros
                                         where a.EsEliminado == NoEliminado && a.GrupoId == grupo
                                         orderby a.Orden ascending
                                         select new Dropdownlist()
                                         {
                                             Id = a.ParametroId,
                                             Value = a.Valor1
                                         }).ToList();

            return result;
        }

        public List<Dropdownlist> GetRoles()
        {
            int grupo = (int)Enumeradores.GrupoParametros.Roles;
            int NoEliminado = (int)Enumeradores.EsEliminado.No;
            List<Dropdownlist> result = (from a in ctx.Parametros
                                         where a.EsEliminado == NoEliminado && a.GrupoId == grupo
                                         orderby a.Orden ascending
                                         select new Dropdownlist()
                                         {
                                             Id = a.ParametroId,
                                             Value = a.Valor1
                                         }).ToList();

            return result;
        }

        public List<Dropdownlist> GetTipoDocumentos()
        {
            int grupo = (int)Enumeradores.GrupoParametros.TipoDocumentos;
            int NoEliminado = (int)Enumeradores.EsEliminado.No;
            List<Dropdownlist> result = (from a in ctx.Parametros
                                         where a.EsEliminado == NoEliminado && a.GrupoId == grupo
                                         orderby a.Orden ascending
                                         select new Dropdownlist()
                                         {
                                             Id = a.ParametroId,
                                             Value = a.Valor1
                                         }).ToList();

            return result;
        }

        public Persona GetPersona(int id)
        {
            int NoEliminado = (int)Enumeradores.EsEliminado.No;
            var data = (from a in ctx.Personas where a.EsEliminado == NoEliminado && a.PersonaId == id select a).FirstOrDefault();
            return data;
        }

        public bool InsertNewPerson(Persona Persona, Usuario Usuario, int UsuarioID)
        {
            try
            {
                if ((from a in ctx.Personas where a.TipoDocumentoId == Persona.TipoDocumentoId && a.NroDocumento == Persona.NroDocumento select a).FirstOrDefault() != null || (from a in ctx.Usuarios where a.NombreUsuario == Usuario.NombreUsuario select a).FirstOrDefault() != null)
                    return false;

                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;

                Persona.UsuGraba = UsuarioID;
                Persona.FechaGraba = DateTime.Now;
                Persona.EsEliminado = NoEsEliminado;
                Persona.GeneroId = Persona.GeneroId == -1 ? 0 : Persona.GeneroId;

                ctx.Personas.Add(Persona);
                int rows = ctx.SaveChanges();

                string pass = Usuario.Contrasenia;

                Usuario.UsuGraba = UsuarioID;
                Usuario.FechaGraba = DateTime.Now;
                Usuario.EsEliminado = NoEsEliminado;
                Usuario.FechaCaduca = DateTime.Now.AddYears(1);
                Usuario.Contrasenia = Utils.Encrypt(Usuario.Contrasenia);
                Usuario.PersonaId = Persona.PersonaId;

                ctx.Usuarios.Add(Usuario);

                rows += ctx.SaveChanges();

                int grupoEmail = (int)Enumeradores.GrupoParametros.Correo;
                int parametroBody = (int)Enumeradores.Correo.BodyEnvioRegistroUsuario;
                int parametroCorreo = (int)Enumeradores.Correo.CorreoSistema;
                int parametroClave = (int)Enumeradores.Correo.ClaveCorreo;
                int parametroHost = (int)Enumeradores.Correo.HostSMTP;

                var parametros = (from a in ctx.Parametros where a.GrupoId == grupoEmail select a).ToList();

                string CorreoSistema = (from a in parametros where a.ParametroId == parametroCorreo select a.Valor2).FirstOrDefault();
                string ClaveCorreo = (from a in parametros where a.ParametroId == parametroClave select a.Valor2).FirstOrDefault();
                string CorreoHost = (from a in parametros where a.ParametroId == parametroHost select a.Valor2).FirstOrDefault();

                string body = (from a in parametros where a.ParametroId == parametroBody select a.Campo).FirstOrDefault();

                body = body.Replace("[@NOMBRE_PERSONA@]", string.Format("{0} {1} {2}", Persona.Nombres, Persona.ApellidoPaterno, Persona.ApellidoMaterno)).Replace("[@NOMBRE_USUARIO@]", Usuario.NombreUsuario).Replace("[@PASSWORD@]", pass);

                string subject = "Registro Exitoso de Usuario";
                List<string> adresses = new List<string>();
                adresses.Add(Persona.Correo);

                Task TASK = new Task(() => Utils.SendMail(body, subject, adresses, CorreoSistema, ClaveCorreo, CorreoHost));
                TASK.Start();

                if (rows > 1)
                    return true;

                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool EditPerson(Persona Persona, Usuario Usuario, int UsuarioID)
        {
            try
            {
                var ctxPersona = (from a in ctx.Personas where a.PersonaId == Persona.PersonaId select a).FirstOrDefault();
                var ctxUsuario = (from a in ctx.Usuarios where a.UsuarioId == Usuario.UsuarioId select a).FirstOrDefault();

                if (ctxPersona == null || ctxUsuario == null)
                    return false;

                ctxPersona.TipoDocumentoId = Persona.TipoDocumentoId;
                ctxPersona.NroDocumento = Persona.NroDocumento;
                ctxPersona.Nombres = Persona.Nombres;
                ctxPersona.ApellidoPaterno = Persona.ApellidoPaterno;
                ctxPersona.ApellidoMaterno = Persona.ApellidoMaterno;
                ctxPersona.FechaNacimiento = Persona.FechaNacimiento;
                ctxPersona.GeneroId = Persona.GeneroId == -1 ? 0 : Persona.GeneroId;
                ctxPersona.Correo = Persona.Correo;
                ctxPersona.Telefono = Persona.Telefono;
                ctxPersona.UsuActualiza = UsuarioID;
                ctxPersona.FechaActualiza = DateTime.Now;


                ctxUsuario.NombreUsuario = Usuario.NombreUsuario;
                if (Utils.Encrypt(Usuario.Contrasenia) != ctxUsuario.Contrasenia && !string.IsNullOrWhiteSpace(Usuario.Contrasenia))
                    ctxUsuario.Contrasenia = Utils.Encrypt(Usuario.Contrasenia);
                ctxUsuario.EmpresaId = Usuario.EmpresaId;
                ctxUsuario.RolId = Usuario.RolId;
                //ctxUsuario.PreguntaSecreta = Usuario.PreguntaSecreta;
                //if (Utils.Encrypt(Usuario.RespuestaSecreta) != ctxUsuario.RespuestaSecreta && !string.IsNullOrWhiteSpace(Usuario.RespuestaSecreta))
                //    ctxUsuario.RespuestaSecreta = Utils.Encrypt(Usuario.RespuestaSecreta);
                ctxUsuario.UsuActualiza = UsuarioID;
                ctxUsuario.FechaActualiza = DateTime.Now;


                int rows = ctx.SaveChanges();

                if (rows > 1)
                    return true;

                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
