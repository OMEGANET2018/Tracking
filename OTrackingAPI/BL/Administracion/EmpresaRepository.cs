using BE.Comun;
using DAL;
using System.Collections.Generic;
using System.Linq;

namespace BL.Administracion
{
    public class EmpresaRepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public List<Dropdownlist> GetEmpresas()
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
    }
}
