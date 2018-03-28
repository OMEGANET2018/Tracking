using BL.Seguimiento;
using BE.Comun;
using System.Web.Http;

namespace OTrackingAPI.Controllers.Seguimiento
{
    public class SeguimientoController : ApiController
    {
        SeguimientoRepository SR = new SeguimientoRepository();

        [HttpPost]
        public IHttpActionResult ObtenerListadoSeguimiento(BandejaSeguimiento data)
        {
            BandejaSeguimiento result = SR.ObtenerListadoSeguimiento(data);
            return Ok(result);
        }
    }
}
