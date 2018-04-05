using BL.Seguimiento;
using System.Collections.Generic;
using BE.Comun;
using System.Web.Http;
using Newtonsoft.Json;

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

        [HttpGet]
        public IHttpActionResult ObtenerTipoControl()
        {
            List<Dropdownlist> result = PR.ObtenerTipoControl();
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GuardarCambiosPlanDeVida(MultiDataModel data)
        {
            PlanesDeVida bandeja = JsonConvert.DeserializeObject<PlanesDeVida>(data.String1);
            bool response = PR.GuardarCambiosComponente(bandeja);
            return Ok(response);
        }

        [HttpPost]
        public IHttpActionResult ObtenerListadoPlanesDeVida(BandejaPlanDeVida data)
        {
            BandejaPlanDeVida response = PR.ObtenerListadoPlanesDeVida(data);
            return Ok(response);
        }

        [HttpGet]
        public IHttpActionResult ObtenerPlanDeVidaPorId(int id)
        {
            PlanesDeVida result = PR.ObtenerPlanDeVidaPorId(id);
            return Ok(result);
        }
    }
}
