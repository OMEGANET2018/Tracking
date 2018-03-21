using BL.Administracion;
using BE.Comun;
using System.Web.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OTrackingAPI.Controllers.Administracion
{
    public class ComponenteController : ApiController
    {
        private ComponenteRepository CR = new ComponenteRepository();

        [HttpPost]
        public IHttpActionResult GuardarCambiosComponente(MultiDataModel data)
        {
            BandejaComponente bandeja = JsonConvert.DeserializeObject<BandejaComponente>(data.String1);
            bool response = CR.GuardarCambiosComponente(bandeja);
            return Ok(response);
        }

        [HttpGet]
        public IHttpActionResult ObtenerListadoComponentes()
        {
            List<BandejaComponente> response = CR.ObtenerListadoComponentes();
            return Ok(response);
        }

        [HttpGet]
        public IHttpActionResult ObtenerListaTipoValorComponentes()
        {
            List<Dropdownlist> response = CR.ObtenerListaTipoValorComponentes();
            return Ok(response);
        }
    }
}