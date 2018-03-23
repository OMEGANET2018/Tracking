using BL.Seguimiento;
using System.Collections.Generic;
using BE.Comun;
using System.Web.Http;

namespace OTrackingAPI.Controllers.Seguimiento
{
    public class PlanDeVidaController : ApiController
    {
        PlanDeVidaRepository PR = new PlanDeVidaRepository();

        [HttpGet]
        public IHttpActionResult ObtenerDxConCIE10(string data)
        {
            var result = PR.ObtenerDxConCIE10(data);

            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult ObtenerTipoTiempo()
        {
            List<Dropdownlist> result = PR.ObtenerTipoTiempo();
            return Ok(result);
        }
    }
}
