using DAL;
using BE.Administracion;
using BE.Comun;
using System.Linq;
using System;
using System.Collections.Generic;

namespace BL.Administracion
{
    public class ComponenteRepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public List<BandejaComponente> ObtenerListadoComponentes()
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                int RecordStatusGrabado = (int)Enumeradores.RecordStatus.Grabado;

                var ListaComponentes = (from a in ctx.Componentes
                                        join b in ctx.Componentes on a.Id equals b.PadreId
                                        where a.PadreId == 0 &&
                                        a.EsEliminado == NoEsEliminado &&
                                        b.EsEliminado == NoEsEliminado
                                        group new { a,b} by a.Id into grp
                                        select new BandejaComponente()
                                        {
                                            Id = grp.FirstOrDefault().a.Id,
                                            EsEliminado = grp.FirstOrDefault().a.EsEliminado,
                                            Nombre = grp.FirstOrDefault().a.Nombre,
                                            RecordStatus = RecordStatusGrabado,
                                            Lista = (from x in grp select new BandejaComponente()
                                                            {
                                                                EsEliminado = x.b.EsEliminado,
                                                                Id = x.b.Id,
                                                                Nombre = x.b.Nombre,
                                                                PadreId = x.b.PadreId,
                                                                RecordStatus = RecordStatusGrabado,
                                                                TipoValorId = x.b.TipoValorId,
                                                                ValorMinimo = x.b.ValorMinimo,
                                                                ValorMaximo = x.b.ValorMaximo
                                                            }).ToList()
                                        }).ToList();

                return ListaComponentes;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public bool GuardarCambiosComponente(BandejaComponente data)
        {
            try
            {
                switch (data.RecordStatus)
                {
                    case (int)Enumeradores.RecordStatus.Agregar:
                        {
                            return AgregarComponente(data);
                        }
                    case (int)Enumeradores.RecordStatus.Eliminar:
                        {
                            return EliminarComponente(data);
                        }
                    case (int)Enumeradores.RecordStatus.Editar:
                    case (int)Enumeradores.RecordStatus.Grabado:
                        {
                            return ActualizarComponente(data);
                        }
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool AgregarComponente(BandejaComponente data)
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;

                Componente Padre = new Componente()
                {
                    EsEliminado = NoEsEliminado,
                    FechaGraba = DateTime.UtcNow,
                    Nombre = data.Nombre,
                    PadreId = 0,
                    UsuGraba = data.UsuGraba
                };

                ctx.Componentes.Add(Padre);
                ctx.SaveChanges();

                if (Padre.Id == 0)
                    return false;

                foreach(Componente C in data.Lista)
                {
                    Componente Hijo = new Componente()
                    {
                        EsEliminado = NoEsEliminado,
                        FechaGraba = DateTime.UtcNow,
                        Nombre = C.Nombre,
                        PadreId = Padre.Id,
                        TipoValorId = C.TipoValorId,
                        ValorMaximo = C.ValorMaximo,
                        ValorMinimo = C.ValorMinimo,
                        UsuGraba = Padre.UsuGraba
                    };

                    ctx.Componentes.Add(Hijo);
                    ctx.SaveChanges();
                }

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool EliminarComponente(BandejaComponente data)
        {
            try
            {
                int EsEliminado = (int)Enumeradores.EsEliminado.Si;

                var ComponentePadre = (from a in ctx.Componentes where a.Id == data.Id select a).FirstOrDefault();

                if (ComponentePadre == null)
                    return false;

                var ComponentesHijos = (from a in ctx.Componentes where a.PadreId == data.Id select a).ToList();

                foreach (var C in ComponentesHijos)
                {
                    C.EsEliminado = EsEliminado;
                    C.UsuActualiza = data.UsuActualiza;
                    C.FechaActualiza = DateTime.UtcNow;
                }

                ComponentePadre.EsEliminado = EsEliminado;
                ComponentePadre.UsuActualiza = data.UsuActualiza;
                ComponentePadre.FechaActualiza = DateTime.UtcNow;

                ctx.SaveChanges();

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool ActualizarComponente(BandejaComponente data)
        {
            try
            {
                int EsEliminado = (int)Enumeradores.EsEliminado.Si;
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                var Padre = (from a in ctx.Componentes where a.Id == data.Id select a).FirstOrDefault();

                if (Padre == null)
                    throw new Exception();

                if(data.RecordStatus == (int)Enumeradores.RecordStatus.Editar)
                {
                    Padre.UsuActualiza = data.UsuActualiza;
                    Padre.FechaActualiza = DateTime.UtcNow;
                    Padre.Nombre = data.Nombre;

                    ctx.SaveChanges();
                }


                foreach(var C in data.Lista)
                {
                    switch (C.RecordStatus)
                    {
                        case (int)Enumeradores.RecordStatus.Agregar:
                            {
                                Componente Hijo = new Componente()
                                {
                                    EsEliminado = NoEsEliminado,
                                    FechaGraba = DateTime.UtcNow,
                                    Nombre = C.Nombre,
                                    PadreId = Padre.Id,
                                    TipoValorId = C.TipoValorId,
                                    UsuGraba = data.UsuActualiza,
                                    ValorMaximo = C.ValorMaximo,
                                    ValorMinimo = C.ValorMinimo
                                };

                                ctx.Componentes.Add(Hijo);
                                ctx.SaveChanges();
                                break;
                            }
                        case (int)Enumeradores.RecordStatus.Eliminar:
                            {
                                var Hijo = (from a in ctx.Componentes where a.Id == C.Id select a).FirstOrDefault();

                                Hijo.EsEliminado = EsEliminado;
                                Hijo.UsuActualiza = data.UsuActualiza;
                                Hijo.FechaActualiza = DateTime.UtcNow;

                                ctx.SaveChanges();
                                break;
                            }
                        case (int)Enumeradores.RecordStatus.Editar:
                            {
                                var Hijo = (from a in ctx.Componentes where a.Id == C.Id select a).FirstOrDefault();

                                Hijo.UsuActualiza = data.UsuActualiza;
                                Hijo.FechaActualiza = DateTime.UtcNow;
                                Hijo.Nombre = C.Nombre;
                                Hijo.TipoValorId = C.TipoValorId;
                                Hijo.ValorMaximo = C.ValorMaximo;
                                Hijo.ValorMinimo = C.ValorMinimo;

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
    }
}
