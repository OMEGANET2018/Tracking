using DAL;
using System;
using System.Collections.Generic;
using BE.Comun;
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
    }
}
