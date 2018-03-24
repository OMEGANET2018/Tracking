using BL.Administracion;
using BE.Comun;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;

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
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult DeleteEmpresa(MultiDataModel data)
        {
            bool result = ER.DeleteEmpresa(data.Int1, data.Int2);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult EditEmpresa(MultiDataModel data)
        {
            BandejaProveedoresLista empresa = JsonConvert.DeserializeObject<BandejaProveedoresLista>(data.String1);
            bool result = ER.EditEmpresa(empresa,data.Int1);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult InsertNewEmpresa(MultiDataModel data)
        {
            BandejaProveedoresLista empresa = JsonConvert.DeserializeObject<BandejaProveedoresLista>(data.String1);
            bool result = ER.InsertNewEmpresa(empresa, data.Int1);
            return Ok(result);
        }
    }
}
