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
                int grupoTipoSeguimiento = (int)Enumeradores.GrupoParametros.TipoSeguimiento;
                int grupoStatusSeguimiento = (int)Enumeradores.GrupoParametros.StatusSeguimiento;
                string Nombre = string.IsNullOrWhiteSpace(data.Nombre) ? "" : data.Nombre;
                int skip = (data.Index - 1) * data.Take;

                var Lista = (from a in ctx.Seguimientos
                             join c in ctx.Personas on a.PersonaId equals c.PersonaId
                             join d in ctx.Parametros on new {a = grupoTipoSeguimiento, b = a.TipoSeguimiento} equals new {a = d.GrupoId, b = d.ParametroId}
                             join e in ctx.Parametros on new { a = grupoStatusSeguimiento, b = a.StatusSeguimiento } equals new { a = e.GrupoId, b = e.ParametroId }
                             where
                             (a.EsEliminado == NoEliminado && c.EsEliminado == NoEliminado) &&
                             (c.Nombres + " " + c.ApellidoPaterno + " " + c.ApellidoMaterno).Contains(Nombre)
                             select new BandejaSeguimientoDetalle()
                             { 
                                Nombre = c.Nombres + " " + c.ApellidoPaterno + " " + c.ApellidoMaterno,
                                Fecha = a.Fecha,
                                TipoSeguimientoId = a.TipoSeguimiento,
                                TipoSeguimiento = d.Valor1,
                                StatusSeguimientoId = a.StatusSeguimiento,
                                StatusSeguimiento = e.Valor1,
                                Color = e.Valor2
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
