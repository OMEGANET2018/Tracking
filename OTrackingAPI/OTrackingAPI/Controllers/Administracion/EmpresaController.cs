using BL.Administracion;
using BE.Comun;
using System.Collections.Generic;
using System.Web.Http;

namespace OTrackingAPI.Controllers.Administracion
{
    public class EmpresaController : ApiController
    {
        private EmpresaRepository ER = new EmpresaRepository();

        [HttpGet]
        public IHttpActionResult GetEmpresas()
        {
            List<Dropdownlist> result = ER.GetEmpresas();
            return Ok(result);
        }
    }
}
