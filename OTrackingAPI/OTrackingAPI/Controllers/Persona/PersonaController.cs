using BL.Personas;
using BE.Comun;
using System.IO;
using System.Web.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using BE.Persona;

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

        [HttpGet]
        public IHttpActionResult GetGeneros()
        {
            List<Dropdownlist> result = PR.GetGeneros();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetRoles()
        {
            List<Dropdownlist> result = PR.GetRoles();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetTipoDocumentos()
        {
            List<Dropdownlist> result = PR.GetTipoDocumentos();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetPersona(int id)
        {
            Persona result = PR.GetPersona(id);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult InsertNewPersona(MultiDataModel data)
        {
            Persona Persona = JsonConvert.DeserializeObject<Persona>(data.String1);
            BE.Acceso.Usuario Usuario = JsonConvert.DeserializeObject<BE.Acceso.Usuario>(data.String2);

            if (string.IsNullOrWhiteSpace(Persona.ApellidoMaterno) || string.IsNullOrWhiteSpace(Persona.ApellidoPaterno) || string.IsNullOrWhiteSpace(Persona.Nombres) || Persona.TipoDocumentoId == -1 || string.IsNullOrWhiteSpace(Persona.NroDocumento) || string.IsNullOrWhiteSpace(Usuario.NombreUsuario) || string.IsNullOrWhiteSpace(Usuario.Contrasenia) || data.Int1 == 0 || Usuario.EmpresaId == -1 || Usuario.RolId == -1)
                return BadRequest("Datos Incompletos");

            bool response = PR.InsertNewPerson(Persona, Usuario, data.Int1);
            return Ok(response);
        }

        [HttpPost]
        public IHttpActionResult EditPersona(MultiDataModel data)
        {
            Persona Persona = JsonConvert.DeserializeObject<Persona>(data.String1);
            BE.Acceso.Usuario Usuario = JsonConvert.DeserializeObject<BE.Acceso.Usuario>(data.String2);

            if (string.IsNullOrWhiteSpace(Persona.ApellidoMaterno) || string.IsNullOrWhiteSpace(Persona.ApellidoPaterno) || string.IsNullOrWhiteSpace(Persona.Nombres) || Persona.TipoDocumentoId == 0 || string.IsNullOrWhiteSpace(Persona.NroDocumento) || string.IsNullOrWhiteSpace(Usuario.NombreUsuario) || data.Int1 == 0 || Usuario.EmpresaId == 0)
                return BadRequest("Datos Incompletos");

            bool response = PR.EditPerson(Persona, Usuario, data.Int1);
            return Ok(response);
        }
    }
}
