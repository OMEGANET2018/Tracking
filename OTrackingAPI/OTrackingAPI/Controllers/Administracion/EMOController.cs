using BL.Administracion;
using BE.Comun;
using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using Newtonsoft.Json;

namespace OTrackingAPI.Controllers.Administracion
{
    public class EMOController : ApiController
    {
        EMORepository ER = new EMORepository();

        [HttpPost]
        public IHttpActionResult ObtenerBandejaEMO(BandejaEMO data)
        {
            var result = ER.ObtenerBandejaEMO(data);

            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult PlantillaEMO(MultiDataModel data)
        {
            BandejaEMO EMO = JsonConvert.DeserializeObject<BandejaEMO>(data.String1);

            string fullPath = HostingEnvironment.MapPath(@"~/Plantillas/PlantillaEMO.xlsx");
            FileStream TemplateFile = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

            MemoryStream response = ER.CrearEMO(TemplateFile,EMO);

            TemplateFile.Dispose();
            return Ok(response);
        }
    }
}
