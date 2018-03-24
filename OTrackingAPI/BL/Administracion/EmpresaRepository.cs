using BE.Comun;
using BE.Administracion;
using DAL;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BL.Administracion
{
    public class EmpresaRepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public List<Dropdownlist> GetEmpresas()
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                List<Dropdownlist> result = (from a in ctx.Empresas
                                             where a.EsEliminado == NoEliminado
                                             select new Dropdownlist()
                                             {
                                                 Id = a.EmpresaId,
                                                 Value = a.RazonSocial
                                             }).ToList();

                return result;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public List<Dropdownlist> GetTipoEmpresa()
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                int grupoTipoEmpresa = (int)Enumeradores.GrupoParametros.TipoEmpresas;
                List<Dropdownlist> result = (from a in ctx.Parametros
                                             where a.GrupoId == grupoTipoEmpresa && a.EsEliminado == NoEliminado
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

        public BandejaProveedoresLista GetEmpresaPorId(int id)
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;

                var data = (from a in ctx.Empresas
                            where a.EsEliminado == NoEliminado && a.EmpresaId == id
                            select new BandejaProveedoresLista()
                            {
                                Email = a.Email,
                                EmpresaId = a.EmpresaId,
                                RazonSocial = a.RazonSocial,
                                Ruc = a.Ruc,
                                TipoEmpresaId = a.TipoEmpresaId
                            }).FirstOrDefault();

                return data;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public BandejaProveedores FiltrarEmpresa(BandejaProveedores data)
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                int grupoTipoEmpresa = (int)Enumeradores.GrupoParametros.TipoEmpresas;
                string RazonSocial = string.IsNullOrWhiteSpace(data.RazonSocial) ? "" : data.RazonSocial;
                string RUC = string.IsNullOrWhiteSpace(data.RUC) ? "" : data.RUC;
                int skip = (data.Index - 1) * data.Take;

                var Listado = (from a in ctx.Empresas
                               join b in ctx.Parametros on new { a = a.TipoEmpresaId, b = grupoTipoEmpresa } equals new { a = b.ParametroId, b = b.GrupoId }
                               where a.EsEliminado == NoEliminado &&
                               a.RazonSocial.Contains(RazonSocial) &&
                               a.Ruc.Contains(RUC)
                               select new BandejaProveedoresLista()
                               {
                                   Email = a.Email,
                                   EmpresaId = a.EmpresaId,
                                   RazonSocial = a.RazonSocial,
                                   Ruc = a.Ruc,
                                   TipoEmpresaId = a.TipoEmpresaId,
                                   TipoEmpresa = b.Valor1
                               }).ToList();

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

        public bool DeleteEmpresa(int empresaid, int userid)
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                var empresa = (from a in ctx.Empresas where a.EmpresaId == empresaid && a.EsEliminado == NoEliminado select a).FirstOrDefault();

                empresa.UsuActualiza = userid;
                empresa.FechaActualiza = DateTime.UtcNow;
                empresa.EsEliminado = (int)Enumeradores.EsEliminado.Si;

                int rows = ctx.SaveChanges();

                return rows > 0;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool EditEmpresa(BandejaProveedoresLista empresa, int usuarioActualiza)
        {
            try
            {
                var proveedor = (from a in ctx.Empresas where a.EmpresaId == empresa.EmpresaId select a).FirstOrDefault();

                if (proveedor == null)
                    return false;

                proveedor.Email = empresa.Email;
                proveedor.FechaActualiza = DateTime.UtcNow;
                proveedor.RazonSocial = empresa.RazonSocial;
                proveedor.Ruc = empresa.Ruc;
                proveedor.TipoEmpresaId = empresa.TipoEmpresaId;
                proveedor.UsuActualiza = usuarioActualiza;

                int rows = ctx.SaveChanges();

                return rows > 0;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool InsertNewEmpresa(BandejaProveedoresLista empresa, int usuarioGraba)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(empresa.Ruc) || string.IsNullOrWhiteSpace(empresa.RazonSocial) || empresa.TipoEmpresaId == -1 || empresa.TipoEmpresaId == 0)
                    return false;

                var proveedor = (from a in ctx.Empresas where a.RazonSocial == empresa.RazonSocial || a.Ruc == empresa.Ruc select a).FirstOrDefault();

                if (proveedor != null)
                    return false;

                Empresa E = new Empresa()
                {
                    Email = empresa.Email,
                    EsEliminado = (int)Enumeradores.EsEliminado.No,
                    FechaGraba = DateTime.UtcNow,
                    RazonSocial = empresa.RazonSocial,
                    Ruc = empresa.Ruc,
                    TipoEmpresaId = empresa.TipoEmpresaId,
                    UsuGraba = usuarioGraba
                };

                ctx.Empresas.Add(E);

                int rows = ctx.SaveChanges();

                return rows > 0;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
