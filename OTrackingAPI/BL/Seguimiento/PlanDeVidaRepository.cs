using DAL;
using System;
using System.Collections.Generic;
using BE.Comun;
using BE.Seguimiento;
using System.Linq;

namespace BL.Seguimiento
{
    public class PlanDeVidaRepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public string ObtenerDxConCIE10(string data)
        {
            try
            {
                var result = (from a in ctx.CIE10 where a.CIE10Id == data select a.Descripcion1).FirstOrDefault();

                return result;
            }
            catch(Exception e)
            {
                return "";
            }
        }

        public List<Dropdownlist> ObtenerTipoTiempo()
        {
            try
            {
                int grupoTipoTiempo = (int)Enumeradores.GrupoParametros.TipoTiempo;
                List<Dropdownlist> result = (from a in ctx.Parametros
                                             where a.GrupoId == grupoTipoTiempo
                                             select new Dropdownlist()
                                             {
                                                 Id = a.ParametroId,
                                                 Value = a.Valor1
                                             }).ToList();

                return result;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public bool GuardarCambiosComponente(PlanesDeVida data)
        {
            try
            {
                switch (data.StatusId)
                {
                    case (int)Enumeradores.RecordStatus.Agregar:
                        {
                            return AgregarPlanDeVida(data);
                        }
                    case (int)Enumeradores.RecordStatus.Eliminar:
                        {
                            return EliminarPlanDeVida(data);
                        }
                    case (int)Enumeradores.RecordStatus.Editar:
                    case (int)Enumeradores.RecordStatus.Grabado:
                        {
                            return ActualizaPlanDeVida(data);
                        }
                }
                return false;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool AgregarPlanDeVida(PlanesDeVida data)
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;

                PlanDeVida PDV = new PlanDeVida()
                {
                    EsEliminado = NoEsEliminado,
                    FechaGraba = DateTime.UtcNow,
                    UsuGraba = data.UsuGraba,
                    Nombre = data.Nombre
                };

                ctx.PlanesDeVida.Add(PDV);
                ctx.SaveChanges();

                if (PDV.PlanesDeVidaId == 0)
                    return false;

                foreach(var Hijo in data.Diagnosticos)
                {
                    DiagnosticoPlanDeVida DPDV = new DiagnosticoPlanDeVida()
                    {
                        EsEliminado = NoEsEliminado,
                        UsuGraba = data.UsuGraba,
                        FechaGraba = DateTime.UtcNow,
                        CIE10Id = Hijo.CIE10,
                        Control = Hijo.Control,
                        ControlNumero = Hijo.ControlNumero,
                        ControlNumeroTipoId = Hijo.ControlNumeroTipoId,
                        ControlRepeticion = Hijo.ControlRepeticion,
                        ControlRepeticionTipoId = Hijo.ControlRepeticionTipoId,
                        Diagnostico = Hijo.Diagnostico,
                        PlanesDeVidaId = PDV.PlanesDeVidaId
                    };

                    ctx.DiagnosticoPlanesDeVida.Add(DPDV);
                    ctx.SaveChanges();
                }

                foreach(var Hijo in data.Citas)
                {
                    CitaPlanDeVida CPDV = new CitaPlanDeVida()
                    {
                        EsEliminado = NoEsEliminado,
                        UsuGraba = data.UsuGraba,
                        FechaGraba = DateTime.UtcNow,
                        Consulta = Hijo.Consulta,
                        Numero = Hijo.Numero,
                        NumeroTipoId = Hijo.NumeroTipoId,
                        Repeticion = Hijo.Repeticion,
                        RepeticionTipoId = Hijo.RepeticionTipoId,
                        PlanesDeVidaId = PDV.PlanesDeVidaId
                    };

                    ctx.CitaPlanesDeVida.Add(CPDV);
                    ctx.SaveChanges();
                }

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool EliminarPlanDeVida(PlanesDeVida data)
        {
            try
            {
                int EsEliminado = (int)Enumeradores.EsEliminado.Si;

                PlanDeVida PDV = (from a in ctx.PlanesDeVida where a.PlanesDeVidaId == data.PlanDeVidaId select a).FirstOrDefault();

                if (PDV == null)
                    return false;

                List<DiagnosticoPlanDeVida> DPDV = (from a in ctx.DiagnosticoPlanesDeVida where a.PlanesDeVidaId == PDV.PlanesDeVidaId select a).ToList();
                List<CitaPlanDeVida> CPDV = (from a in ctx.CitaPlanesDeVida where a.PlanesDeVidaId == PDV.PlanesDeVidaId select a).ToList();

                foreach (var Hijo in DPDV)
                {
                    Hijo.UsuActualiza = data.UsuGraba;
                    Hijo.FechaActualiza = DateTime.UtcNow;
                    Hijo.EsEliminado = EsEliminado;
                }

                foreach (var Hijo in CPDV)
                {
                    Hijo.UsuActualiza = data.UsuGraba;
                    Hijo.FechaActualiza = DateTime.UtcNow;
                    Hijo.EsEliminado = EsEliminado;
                }

                PDV.UsuActualiza = data.UsuGraba;
                PDV.FechaActualiza = DateTime.UtcNow;
                PDV.EsEliminado = EsEliminado;

                ctx.SaveChanges();

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool ActualizaPlanDeVida(PlanesDeVida data)
        {
            try
            {
                int EsEliminado = (int)Enumeradores.EsEliminado.Si;
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                PlanDeVida PDV = (from a in ctx.PlanesDeVida where a.PlanesDeVidaId == data.PlanDeVidaId select a).FirstOrDefault();

                if (PDV == null)
                    throw new Exception();

                if(data.StatusId == (int)Enumeradores.RecordStatus.Editar)
                {
                    PDV.UsuActualiza = data.UsuGraba;
                    PDV.FechaActualiza = DateTime.UtcNow;
                    PDV.Nombre = data.Nombre;

                    ctx.SaveChanges();
                }

                foreach(var Hijo in data.Diagnosticos)
                {
                    switch (Hijo.StatusId)
                    {
                        case (int)Enumeradores.RecordStatus.Agregar:
                            {
                                DiagnosticoPlanDeVida DPDV = new DiagnosticoPlanDeVida()
                                {
                                    EsEliminado = NoEsEliminado,
                                    UsuGraba = data.UsuGraba,
                                    FechaGraba = DateTime.UtcNow,
                                    CIE10Id = Hijo.CIE10,
                                    Control = Hijo.Control,
                                    ControlNumero = Hijo.ControlNumero,
                                    ControlNumeroTipoId = Hijo.ControlNumeroTipoId,
                                    ControlRepeticion = Hijo.ControlRepeticion,
                                    ControlRepeticionTipoId = Hijo.ControlRepeticionTipoId,
                                    Diagnostico = Hijo.Diagnostico,
                                    PlanesDeVidaId = PDV.PlanesDeVidaId
                                };

                                ctx.DiagnosticoPlanesDeVida.Add(DPDV);
                                ctx.SaveChanges();
                                break;
                            }
                        case (int)Enumeradores.RecordStatus.Eliminar:
                            {
                                var DPDV = (from a in ctx.DiagnosticoPlanesDeVida where a.DiagnosticoPlanesDeVidaId == Hijo.DiagnosticoId select a).FirstOrDefault();

                                DPDV.UsuActualiza = data.UsuGraba;
                                DPDV.FechaActualiza = DateTime.UtcNow;
                                DPDV.EsEliminado = EsEliminado;

                                ctx.SaveChanges();
                                break;
                            }
                        case (int)Enumeradores.RecordStatus.Editar:
                            {
                                var DPDV = (from a in ctx.DiagnosticoPlanesDeVida where a.DiagnosticoPlanesDeVidaId == Hijo.DiagnosticoId select a).FirstOrDefault();

                                DPDV.UsuActualiza = data.UsuGraba;
                                DPDV.FechaActualiza = DateTime.UtcNow;
                                DPDV.CIE10Id = Hijo.CIE10;
                                DPDV.Control = Hijo.Control;
                                DPDV.ControlNumero = Hijo.ControlNumero;
                                DPDV.ControlNumeroTipoId = Hijo.ControlNumeroTipoId;
                                DPDV.ControlRepeticion = Hijo.ControlRepeticion;
                                DPDV.ControlRepeticionTipoId = Hijo.ControlRepeticionTipoId;
                                DPDV.Diagnostico = Hijo.Diagnostico;

                                ctx.SaveChanges();
                                break;
                            }
                    }
                }

                foreach (var Hijo in data.Citas)
                {
                    switch (Hijo.StatusId)
                    {
                        case (int)Enumeradores.RecordStatus.Agregar:
                            {
                                CitaPlanDeVida CPDV = new CitaPlanDeVida()
                                {
                                    EsEliminado = NoEsEliminado,
                                    UsuGraba = data.UsuGraba,
                                    FechaGraba = DateTime.UtcNow,
                                    Consulta = Hijo.Consulta,
                                    Numero = Hijo.Numero,
                                    NumeroTipoId = Hijo.NumeroTipoId,
                                    Repeticion = Hijo.Repeticion,
                                    RepeticionTipoId = Hijo.RepeticionTipoId,
                                    PlanesDeVidaId = PDV.PlanesDeVidaId
                                };

                                ctx.CitaPlanesDeVida.Add(CPDV);
                                ctx.SaveChanges();
                                break;
                            }
                        case (int)Enumeradores.RecordStatus.Eliminar:
                            {
                                var CPDV = (from a in ctx.CitaPlanesDeVida where a.CitaPlanesDeVidaId == Hijo.CitaId select a).FirstOrDefault();

                                CPDV.UsuActualiza = data.UsuGraba;
                                CPDV.FechaActualiza = DateTime.UtcNow;
                                CPDV.EsEliminado = EsEliminado;

                                ctx.SaveChanges();
                                break;
                            }
                        case (int)Enumeradores.RecordStatus.Editar:
                            {
                                var CPDV = (from a in ctx.CitaPlanesDeVida where a.CitaPlanesDeVidaId == Hijo.CitaId select a).FirstOrDefault();

                                CPDV.UsuActualiza = data.UsuGraba;
                                CPDV.FechaActualiza = DateTime.UtcNow;
                                CPDV.Consulta = Hijo.Consulta;
                                CPDV.Numero = Hijo.Numero;
                                CPDV.NumeroTipoId = Hijo.NumeroTipoId;
                                CPDV.Repeticion = Hijo.Repeticion;
                                CPDV.RepeticionTipoId = Hijo.RepeticionTipoId;

                                ctx.SaveChanges();
                                break;
                            }
                    }
                }

                ctx.Database.CurrentTransaction.Commit();
                return true;
            }
            catch(Exception e)
            {
                ctx.Database.CurrentTransaction.Rollback();
                return false;
            }
        }

        public BandejaPlanDeVida ObtenerListadoPlanesDeVida(BandejaPlanDeVida data)
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                int RecordStatusGrabado = (int)Enumeradores.RecordStatus.Grabado;
                int skip = (data.Index - 1) * data.Take;

                var Listado = (from a in ctx.PlanesDeVida where a.EsEliminado == NoEsEliminado
                               select new PlanesDeVida()
                               {
                                   PlanDeVidaId = a.PlanesDeVidaId,
                                   StatusId = RecordStatusGrabado,
                                   Nombre = a.Nombre
                               }).ToList();

                var Diagnosticos = (from a in ctx.DiagnosticoPlanesDeVida where a.EsEliminado == NoEsEliminado select a).ToList();
                var Citas = (from a in ctx.CitaPlanesDeVida where a.EsEliminado == NoEsEliminado select a).ToList();

                foreach(var L in Listado)
                {
                    L.Diagnosticos = (from a in Diagnosticos where a.PlanesDeVidaId == L.PlanDeVidaId
                                      select new DiagnosticoPlanesDeVida()
                                      {
                                          CIE10 = a.CIE10Id,
                                          Control = a.Control,
                                          ControlNumero = a.ControlNumero,
                                          ControlNumeroTipoId = a.ControlNumeroTipoId,
                                          ControlRepeticion = a.ControlRepeticion,
                                          ControlRepeticionTipoId = a.ControlRepeticionTipoId,
                                          Diagnostico = a.Diagnostico,
                                          DiagnosticoId = a.DiagnosticoPlanesDeVidaId,
                                          StatusId = RecordStatusGrabado
                                      }).ToList();

                    L.Citas = (from a in Citas
                               where a.PlanesDeVidaId == L.PlanDeVidaId
                               select new CitaPlanesDeVida()
                               {
                                   CitaId = a.CitaPlanesDeVidaId,
                                   Consulta = a.Consulta,
                                   Numero = a.Numero,
                                   NumeroTipoId = a.NumeroTipoId,
                                   Repeticion = a.Repeticion,
                                   RepeticionTipoId = a.RepeticionTipoId,
                                   StatusId = RecordStatusGrabado
                               }).ToList();
                }

                int TotalRegistros = Listado.Count;

                if (data.Take > 0)
                    Listado = Listado.Skip(skip).Take(data.Take).ToList();

                data.TotalRegistros = TotalRegistros;
                data.Lista = Listado;

                return data;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public PlanesDeVida ObtenerPlanDeVidaPorId(int id)
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                int RecordStatusGrabado = (int)Enumeradores.RecordStatus.Grabado;

                PlanesDeVida data = (from a in ctx.PlanesDeVida
                                     where a.PlanesDeVidaId == id && a.EsEliminado == NoEsEliminado
                                     select new PlanesDeVida()
                                     {
                                         Nombre = a.Nombre,
                                         PlanDeVidaId = a.PlanesDeVidaId,
                                         StatusId = RecordStatusGrabado
                                     }).FirstOrDefault();

                data.Diagnosticos = (from a in ctx.DiagnosticoPlanesDeVida
                                     where a.PlanesDeVidaId == data.PlanDeVidaId && a.EsEliminado == NoEsEliminado
                                     select new DiagnosticoPlanesDeVida()
                                     {
                                         CIE10 = a.CIE10Id,
                                         Control = a.Control,
                                         ControlNumero = a.ControlNumero,
                                         ControlNumeroTipoId = a.ControlNumeroTipoId,
                                         ControlRepeticion = a.ControlRepeticion,
                                         ControlRepeticionTipoId = a.ControlRepeticionTipoId,
                                         Diagnostico = a.Diagnostico,
                                         DiagnosticoId = a.DiagnosticoPlanesDeVidaId,
                                         StatusId = RecordStatusGrabado
                                     }).ToList();

                data.Citas = (from a in ctx.CitaPlanesDeVida
                              where a.PlanesDeVidaId == data.PlanDeVidaId && a.EsEliminado == NoEsEliminado
                              select new CitaPlanesDeVida()
                              {
                                  CitaId = a.CitaPlanesDeVidaId,
                                  Consulta = a.Consulta,
                                  Numero = a.Numero,
                                  NumeroTipoId = a.NumeroTipoId,
                                  Repeticion = a.Repeticion,
                                  RepeticionTipoId = a.RepeticionTipoId,
                                  StatusId = RecordStatusGrabado
                              }).ToList();

                return data;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
