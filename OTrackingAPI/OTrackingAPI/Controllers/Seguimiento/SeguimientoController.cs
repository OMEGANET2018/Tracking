using BL.Seguimiento;
using BE.Comun;
using System.Web.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Web.Hosting;

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

        [HttpGet]
        public IHttpActionResult ConfirmarSeguimiento(int seguimientoId, int UserId)
        {
            bool response = SR.ConfirmarSeguimiento(seguimientoId,UserId);
            return Ok(response);
        }

        [HttpGet]
        public IHttpActionResult EliminarSeguimiento(int seguimientoId, int UserId)
        {
            bool response = SR.EliminarSeguimiento(seguimientoId,UserId);
            return Ok(response);
        }

        [HttpGet]
        public IHttpActionResult NotificarSeguimiento(string json)
        {
            List<int> SeguimientoIdList = JsonConvert.DeserializeObject<List<int>>(json);

            string response = SR.NotificarSeguimiento(SeguimientoIdList);
            return Ok(response);
        }

        [HttpGet]
        public IHttpActionResult CambiarSeguimiento(int seguimientoId, string fechaString, int UserId)
        {
            bool response = SR.CambiarSeguimiento(seguimientoId, fechaString, UserId);
            return Ok(response);
        }

        [HttpPost]
        public IHttpActionResult AdjuntarCita (MultiDataModel data)
        {
            string FolderPath = System.Web.Configuration.WebConfigurationManager.AppSettings["ArchivosConsulta"].ToString();
            string LocalPath = HostingEnvironment.MapPath(FolderPath);

            bool response = SR.AdjuntarCita(data.Int1, data.String1, data.String2, data.Int2, LocalPath);
            return Ok(response);
        }
    }
}
