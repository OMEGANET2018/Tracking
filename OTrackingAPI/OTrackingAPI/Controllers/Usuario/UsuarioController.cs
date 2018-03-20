using BE.Acceso;
using BE.Comun;
using BL.Acceso;
using System.Collections.Generic;
using System.Web.Http;

namespace OTrackingAPI.Controllers.Usuario
{
    public class UsuarioController : ApiController
    {
        private UsuarioRepositorio UR = new UsuarioRepositorio();

        [HttpGet]
        public IHttpActionResult GetUsuario(string usuario, string contrasenia)
        {
            UsuarioAutorizado result = UR.LoginUsuario(usuario, contrasenia);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetAutorizacion(int rolId)
        {
            if (rolId == 0)
                return BadRequest("Rol Inválido");

            List<Autorizacion> result = UR.GetAutorizacion(rolId);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetUsuarios(BandejaUsuario data)
        {
            BandejaUsuario result = UR.GetUsuarios(data);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetUsuario(int id)
        {
            BE.Acceso.Usuario result = UR.GetUsuario(id);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult DeleteUser(MultiDataModel data)
        {
            if (data.Int1 == 0)
                return BadRequest("Información Inválida");

            if (data.Int2 == 0)
                return BadRequest("Sesión Expirada");

            bool response = UR.DeleteUser(data.Int1, data.Int2);
            return Ok(response);
        }

        [HttpGet]
        public IHttpActionResult GetTreeView(int id)
        {
            List<TreeView> result = UR.GetTreeData(id);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        public IHttpActionResult InsertRol(MultiDataModel data)
        {
            if (string.IsNullOrWhiteSpace(data.String1))
                return BadRequest("Nombre Inválido");

            if (data.Int1 == 0)
                return BadRequest("Sesión Expirada");

            Parametro response = UR.InsertRol(data.String1, Newtonsoft.Json.JsonConvert.DeserializeObject<List<TreeView>>(data.String2), data.Int1);
            return Ok(response);
        }
    }
}
