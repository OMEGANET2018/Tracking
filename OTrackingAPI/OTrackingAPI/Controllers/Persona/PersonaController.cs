using BL;
using System.Web.Http;

namespace OTrackingAPI.Controllers
{
    public class PersonaController : ApiController
    {
        private PersonaRepository PR = new PersonaRepository();

        [HttpGet]
        public IHttpActionResult Index()
        {
            return Ok("Conexion Exitosa!");
        }
    }
}
