using DAL;
using BE.Comun;
using System;
using System.Linq;

namespace BL.Seguimiento
{
    public class SeguimientoRepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public BandejaSeguimiento ObtenerListadoSeguimiento(BandejaSeguimiento data)
        {
            try
            {
                int NoEliminado = (int)Enumeradores.EsEliminado.No;
                string Nombre = string.IsNullOrWhiteSpace(data.Nombre) ? "" : data.Nombre;
                int skip = (data.Index - 1) * data.Take;

                var Lista = (from a in ctx.FechasSeguimiento
                             select new BandejaSeguimientoDetalle()
                             { 
                                
                             }).ToList();

                int TotalRegistros = Lista.Count;

                if (data.Take > 0)
                    Lista = Lista.Skip(skip).Take(data.Take).ToList();

                data.TotalRegistros = TotalRegistros;
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
