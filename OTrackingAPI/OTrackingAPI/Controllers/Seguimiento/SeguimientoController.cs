using BL.Seguimiento;
using BE.Comun;
using System.Web.Http;

namespace OTrackingAPI.Controllers.Seguimiento
{
    public class SeguimientoController : ApiController
    {
        SeguimientoRepository SR = new SeguimientoRepository();

        [HttpPost]
        public IHttpActionResult ObtenerBandejaGenerarPlantilla(BandejaGenerarPlantilla data)
        {
            BandejaGenerarPlantilla response = SR.ObtenerBandejaGenerarPlantilla(data);
            return Ok();
        }
    }
}
