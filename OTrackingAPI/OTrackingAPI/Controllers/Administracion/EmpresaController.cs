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

        [HttpGet]
        public IHttpActionResult GetTipoEmpresa()
        {
            List<Dropdownlist> result = ER.GetTipoEmpresa();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetEmpresaPorId(int id)
        {
            BandejaProveedoresLista result = ER.GetEmpresaPorId(id);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult FiltrarEmpresa(BandejaProveedores data)
        {
            var result = ER.FiltrarEmpresa(data);
            return Ok();
        }
    }
}
