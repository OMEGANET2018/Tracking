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
                int grupoParametro = (int)Enumeradores.GrupoParametros.TipoValorComponente;

                var componentes = (from a in ctx.Componentes where a.EsEliminado == NoEsEliminado select a).ToList();
                var parametros = (from a in ctx.Parametros where a.GrupoId == grupoParametro && a.EsEliminado == NoEsEliminado select a).ToList();

                var ListaComponentes = (from a in componentes
                                        where a.PadreId == 0
                                        group new { a } by a.ComponenteId into grp
                                        select new BandejaComponente()
                                        {
                                            ComponenteId = grp.FirstOrDefault().a.ComponenteId,
                                            EsEliminado = grp.FirstOrDefault().a.EsEliminado,
                                            Nombre = grp.FirstOrDefault().a.Nombre,
                                            RecordStatus = RecordStatusGrabado,
                                            Lista = (from a in componentes where a.PadreId == grp.FirstOrDefault().a.ComponenteId
                                                     select new BandejaComponente()
                                                     {
                                                         EsEliminado = a.EsEliminado,
                                                         ComponenteId = a.ComponenteId,
                                                         Nombre = a.Nombre,
                                                         PadreId = a.PadreId,
                                                         RecordStatus = RecordStatusGrabado,
                                                         TipoValorId = a.TipoValorId,
                                                         TipoValorTexto = parametros.Where(z => z.ParametroId == a.TipoValorId).Select(z => z.Valor1).FirstOrDefault(),
                                                         ValorMinimo = a.ValorMinimo,
                                                         ValorMaximo = a.ValorMaximo
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

                if (Padre.ComponenteId == 0)
                    return false;

                if(data.Lista != null)
                {
                    foreach (BandejaComponente C in data.Lista)
                    {
                        Componente Hijo = new Componente()
                        {
                            EsEliminado = NoEsEliminado,
                            FechaGraba = DateTime.UtcNow,
                            Nombre = C.Nombre,
                            PadreId = Padre.ComponenteId,
                            TipoValorId = C.TipoValorId,
                            ValorMaximo = C.ValorMaximo,
                            ValorMinimo = C.ValorMinimo,
                            UsuGraba = Padre.UsuGraba
                        };

                        ctx.Componentes.Add(Hijo);
                        ctx.SaveChanges();
                    }
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

                var ComponentePadre = (from a in ctx.Componentes where a.ComponenteId == data.ComponenteId select a).FirstOrDefault();

                if (ComponentePadre == null)
                    return false;

                var ComponentesHijos = (from a in ctx.Componentes where a.PadreId == data.ComponenteId select a).ToList();

                foreach (var C in ComponentesHijos)
                {
                    C.EsEliminado = EsEliminado;
                    C.UsuActualiza = data.UsuGraba;
                    C.FechaActualiza = DateTime.UtcNow;
                }

                ComponentePadre.EsEliminado = EsEliminado;
                ComponentePadre.UsuActualiza = data.UsuGraba;
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

                var Padre = (from a in ctx.Componentes where a.ComponenteId == data.ComponenteId select a).FirstOrDefault();

                if (Padre == null)
                    throw new Exception();

                if(data.RecordStatus == (int)Enumeradores.RecordStatus.Editar)
                {
                    Padre.UsuActualiza = data.UsuGraba;
                    Padre.FechaActualiza = DateTime.UtcNow;
                    Padre.Nombre = data.Nombre;

                    ctx.SaveChanges();
                }

                if(data.Lista != null)
                {
                    foreach (var C in data.Lista)
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
                                        PadreId = Padre.ComponenteId,
                                        TipoValorId = C.TipoValorId,
                                        UsuGraba = data.UsuGraba,
                                        ValorMaximo = C.ValorMaximo,
                                        ValorMinimo = C.ValorMinimo
                                    };

                                    ctx.Componentes.Add(Hijo);
                                    ctx.SaveChanges();
                                    break;
                                }
                            case (int)Enumeradores.RecordStatus.Eliminar:
                                {
                                    var Hijo = (from a in ctx.Componentes where a.ComponenteId == C.ComponenteId select a).FirstOrDefault();

                                    Hijo.EsEliminado = EsEliminado;
                                    Hijo.UsuActualiza = data.UsuGraba;
                                    Hijo.FechaActualiza = DateTime.UtcNow;

                                    ctx.SaveChanges();
                                    break;
                                }
                            case (int)Enumeradores.RecordStatus.Editar:
                                {
                                    var Hijo = (from a in ctx.Componentes where a.ComponenteId == C.ComponenteId select a).FirstOrDefault();

                                    Hijo.UsuActualiza = data.UsuGraba;
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

        public List<Dropdownlist> ObtenerListaTipoValorComponentes()
        {
            int grupoParametro = (int)Enumeradores.GrupoParametros.TipoValorComponente;
            int NoEsEliminado = (int)Enumeradores.EsEliminado.No;

            var data = (from a in ctx.Parametros
                        where a.GrupoId == grupoParametro && a.EsEliminado == NoEsEliminado
                        select new Dropdownlist()
                        {
                            Id = a.ParametroId,
                            Value = a.Valor1
                        }).ToList();

            return data;
        }
    }
}
