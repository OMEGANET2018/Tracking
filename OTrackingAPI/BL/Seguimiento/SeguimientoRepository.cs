using BE.Comun;
using DAL;
using System;
using System.Linq;

namespace BL.Seguimiento
{
    public class SeguimientoRepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public BandejaGenerarPlantilla ObtenerBandejaGenerarPlantilla(BandejaGenerarPlantilla data)
        {
            try
            {
                int NoEsEliminado = (int)Enumeradores.EsEliminado.No;

                var Lista = (from a in ctx.Personas
                             where a.EsEliminado == NoEsEliminado
                             select new BandejaGenerarPlantillaLista()
                             {
                                 Nombre = a.Nombres + " " + a.ApellidoPaterno + " " + a.ApellidoMaterno
                             }).ToList();

                data.Lista = Lista;

                return data;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
