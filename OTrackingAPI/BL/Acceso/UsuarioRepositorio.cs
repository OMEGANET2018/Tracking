using BE.Acceso;
using BE.Comun;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Acceso
{
    public class UsuarioRepositorio
    {
        private DatabaseContext ctx = new DatabaseContext();

        public UsuarioAutorizado LoginUsuario(string usuario, string contrasenia)
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
                UsuarioAutorizado oUsuarioAutorizado = new UsuarioAutorizado();
                var qUsuario = (from a in ctx.Usuarios
                                join b in ctx.Personas on a.PersonaId equals b.PersonaId
                                join c in ctx.Parametros on new { a = a.RolId, b = 100 } equals new { a = c.ParametroId, b = c.GrupoId }
                                where a.NombreUsuario == usuario && 
                                    a.Contrasenia == contrasenia &&
                                    a.EsEliminado == NoEsEliminado
                                select new UsuarioAutorizado
                                {
                                    UsuarioId = a.UsuarioId,
                                    PersonaId = a.PersonaId,
                                    EmpresaId = a.EmpresaId,
                                    NombreUsuario = a.NombreUsuario,
                                    NombreCompleto = b.Nombres + " " + b.ApellidoPaterno,
                                    FechaCaduca = a.FechaCaduca.Value,
                                    RolId = a.RolId,
                                    foto = b.Foto,
                                    Rol = c.Valor1

                                }).FirstOrDefault();
                if (qUsuario != null)
                {
                    var qAutu = GetAutorizacion(qUsuario.RolId);
                    qUsuario.Autorizacion = qAutu;
                }
                else
                {
                    return null;
                }

                return qUsuario;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Autorizacion> GetAutorizacion(int rolId)
        {
            int NoEsEliminado = (int)Enumeradores.EsEliminado.No;
            var perfil = (from a in ctx.Perfiles
                          join b in ctx.Menus on a.MenuId equals b.MenuId
                          where a.RolId == rolId && a.EsEliminado == NoEsEliminado
                          select new SubMenu
                          {
                              MenuId = a.MenuId,
                              Descripcion = b.Descripcion,
                              PadreId = b.PadreId,
                              Icono = b.Icono,
                              Uri = b.Uri
                          }).ToList();

            var query = (from a in ctx.Perfiles
                         join b in ctx.Menus on a.MenuId equals b.MenuId
                         where a.RolId == rolId && a.EsEliminado == NoEsEliminado && b.PadreId == -1
                         select new Autorizacion
                         {
                             PerfilId = a.PerfilId,
                             RolId = a.RolId,
                             MenuId = b.MenuId,
                             Descripcion = b.Descripcion,
                             PadreId = b.PadreId,
                             Icono = b.Icono,
                             Uri = b.Uri
                         }).ToList();
            query.ForEach(a =>
            {
                a.SubMenus = perfil.FindAll(p => p.PadreId == a.MenuId);
            });

            return query;


        }

        public BandejaUsuario GetUsuarios(BandejaUsuario data)
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                int GrupoRol = (int)Enumeradores.GrupoParametros.Roles;
                int GrupoEmpresa = (int)Enumeradores.GrupoParametros.TipoEmpresas;
                string NombreUsuario = string.IsNullOrWhiteSpace(data.NombreUsuario) ? "" : data.NombreUsuario;
                string NombrePersona = string.IsNullOrWhiteSpace(data.NombrePersona) ? "" : data.NombrePersona;
                int skip = (data.Index - 1) * data.Take;

                var Lista = (from U in ctx.Usuarios
                             join P in ctx.Personas on U.PersonaId equals P.PersonaId
                             join R in ctx.Parametros on U.RolId equals R.ParametroId
                             join E in ctx.Empresas on U.EmpresaId equals E.EmpresaId
                             join TE in ctx.Parametros on E.TipoEmpresaId equals TE.ParametroId
                             where U.EsEliminado == NoEliminado &&
                             P.EsEliminado == NoEliminado &&
                             R.GrupoId == GrupoRol &&
                             TE.GrupoId == GrupoEmpresa &&
                             U.NombreUsuario.Contains(NombreUsuario) &&
                             (P.Nombres.Contains(NombrePersona) ||
                             P.ApellidoPaterno.Contains(NombrePersona) ||
                             P.ApellidoMaterno.Contains(NombrePersona))
                             select new BandejaUsuarioLista()
                             {
                                 UsuarioId = U.UsuarioId,
                                 NombreCompleto = P.Nombres + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno,
                                 NombreUsuario = U.NombreUsuario,
                                 Empresa = E.RazonSocial,
                                 Rol = R.Valor1,
                                 TipoEmpresa = TE.Valor1
                             }).ToList();

                int TotalRegistros = Lista.Count;

                if (data.Take > 0)
                    Lista = Lista.Skip(skip).Take(data.Take).ToList();

                data.TotalRegistros = TotalRegistros;
                data.Lista = Lista;

                return data;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Usuario GetUsuario(int id)
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                return (from a in ctx.Usuarios
                        where a.EsEliminado == NoEliminado && a.UsuarioId == id
                        select a).ToList().Select(a => new Usuario()
                        {
                            EmpresaId = a.EmpresaId,
                            EsEliminado = a.EsEliminado,
                            FechaActualiza = a.FechaActualiza,
                            FechaCaduca = a.FechaCaduca,
                            PersonaId = a.PersonaId,
                            FechaGraba = a.FechaGraba,
                            NombreUsuario = a.NombreUsuario,
                            PreguntaSecreta = a.PreguntaSecreta,
                            RolId = a.RolId,
                            UsuActualiza = a.UsuActualiza,
                            UsuarioId = a.UsuarioId,
                            UsuGraba = a.UsuGraba
                        }).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool DeleteUser(int DeleteId, int UserId)
        {
            try
            {
                var Usuario = (from a in ctx.Usuarios where a.UsuarioId == DeleteId select a).FirstOrDefault();

                Usuario.UsuActualiza = UserId;
                Usuario.FechaActualiza = DateTime.Now;
                Usuario.EsEliminado = (int)Enumeradores.EsEliminado.Si;

                int rows = ctx.SaveChanges();
                return rows > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<TreeView> GetTreeData(int id)
        {
            try
            {
                var menus = (from a in ctx.Menus select a).ToList();
                List<int> accesos = (from a in ctx.Perfiles where a.RolId == id select a.MenuId).ToList();

                List<TreeView> FatherList = new List<TreeView>();
                foreach (var padre in menus.Where(x => x.PadreId == -1))
                {
                    List<TreeView> ChildList = new List<TreeView>();
                    foreach (var hijo in menus.Where(x => x.PadreId == padre.MenuId))
                    {
                        TreeView Child = new TreeView()
                        {
                            text = hijo.Descripcion,
                            state = new TreeViewState()
                            {
                                @checked = accesos.Contains(hijo.MenuId)
                            },
                            MenuId = hijo.MenuId
                        };
                        ChildList.Add(Child);
                    }
                    TreeView Father = new TreeView()
                    {
                        text = padre.Descripcion,
                        nodes = ChildList.ToArray(),
                        state = new TreeViewState()
                        {
                            @checked = accesos.Contains(padre.MenuId)
                        },
                        MenuId = padre.MenuId
                    };
                    FatherList.Add(Father);
                }


                return FatherList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Parametro InsertRol(string Nombre, List<TreeView> Tree, int UserID)
        {
            try
            {
                int rows = 0;
                int grupo = (int)Enumeradores.GrupoParametros.Roles;
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                List<Parametro> Listado = (from a in ctx.Parametros where a.GrupoId == grupo select a).ToList();
                int parametroId = (from a in Listado orderby a.ParametroId descending select a.ParametroId).FirstOrDefault() + 1;
                int? orden = (from a in Listado orderby a.Orden descending select a.Orden).FirstOrDefault();

                orden = orden.HasValue ? orden.Value + 1 : 1;

                Parametro Parametro = (from a in Listado where a.Valor1 == Nombre select a).FirstOrDefault();

                if (Parametro == null)
                {
                    Parametro = new Parametro()
                    {
                        GrupoId = grupo,
                        ParametroId = parametroId,
                        Valor1 = Nombre,
                        PadreParametroId = -1,
                        Orden = orden,
                        EsEliminado = NoEliminado,
                        FechaGraba = DateTime.Now,
                        UsuGraba = UserID
                    };

                    ctx.Parametros.Add(Parametro);
                    rows = rows + ctx.SaveChanges();
                }


                List<Perfil> ListPerfiles = (from a in ctx.Perfiles where a.RolId == Parametro.ParametroId select a).ToList();

                if (ListPerfiles != null)
                {
                    if (ListPerfiles.Count > 0)
                    {
                        ctx.Perfiles.RemoveRange(ListPerfiles);
                        ctx.SaveChanges();
                    }
                }

                ListPerfiles = new List<Perfil>();

                foreach (var P in Tree)
                {
                    Perfil perfil = new Perfil()
                    {
                        RolId = Parametro.ParametroId,
                        MenuId = P.MenuId,
                        EsEliminado = NoEliminado,
                        FechaGraba = DateTime.Now,
                        UsuGraba = UserID
                    };

                    ListPerfiles.Add(perfil);
                }


                ctx.Perfiles.AddRange(ListPerfiles);


                rows = rows + ctx.SaveChanges();
                if (rows > 0)
                    return Parametro;


                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
