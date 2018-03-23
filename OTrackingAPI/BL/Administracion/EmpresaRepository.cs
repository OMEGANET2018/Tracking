using BE.Comun;
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
    }
}
