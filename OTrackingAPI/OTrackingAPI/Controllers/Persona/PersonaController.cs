using BL.Persona;
using System.IO;
using System.Web.Http;
using Newtonsoft.Json;

namespace OTrackingAPI.Controllers
{
    public class PersonaController : ApiController
    {
        private PersonaRepository PR = new PersonaRepository();

        [HttpPost]
        public IHttpActionResult InsertarPersonaExcel()
        {
            System.Threading.Tasks.Task<byte[]> bytes = Request.Content.ReadAsByteArrayAsync();

            MemoryStream stream = new MemoryStream(bytes.Result);
            object response = PR.InsertarPersonaExcel(stream);

            return Ok(JsonConvert.SerializeObject(response));
        }
    }
}
