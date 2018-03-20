using BL.Administracion;
using BE.Comun;
using System.Web.Http;
using System.Collections.Generic;

namespace OTrackingAPI.Controllers.Administracion
{
    public class ComponenteController : ApiController
    {
        private ComponenteRepository CR = new ComponenteRepository();

        [HttpPost]
        public IHttpActionResult GuardarCambiosComponente(BandejaComponente data)
        {
            bool response = CR.GuardarCambiosComponente(data);
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