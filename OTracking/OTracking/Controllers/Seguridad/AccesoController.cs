using OTracking.Models;
using OTracking.Models.Comun;
using OTracking.Utils;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OTracking.Controllers.Seguridad
{
    public class AccesoController : Controller
    {
        [GeneralSecurity(Rol = "Seguridad-Usuario de Sistema")]
        public ActionResult BandejaUsuarios()
        {
            Api API = new Api();
            ViewBag.Usuarios = new BandejaUsuario() { Lista = new List<BandejaUsuarioLista>(), Take = 10 };
            return View();
        }

        [GeneralSecurity(Rol = "Seguridad-Usuario de Sistema")]
        public ActionResult FiltrarUsuario(BandejaUsuario data)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "NombrePersona",data.NombrePersona },
                { "NombreUsuario", data.NombreUsuario},
                { "Index", data.Index.ToString()},
                { "Take", data.Take.ToString()}
            };
            ViewBag.Usuarios = API.Post<BandejaUsuario>("Usuario/GetUsuarios", arg);
            return PartialView("_BandejaUsuariosPartial");
        }

        [GeneralSecurity(Rol = "Seguridad-Usuario de Sistema")]
        public JsonResult DeleteUser(int id)
        {
            Api API = new Api();
            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "Int1", id.ToString() },
                { "Int2", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Post<bool>("Usuario/DeleteUser", args);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Seguridad-Usuario de Sistema")]
        public ActionResult CrearUsuario(int? id)
        {
            Api API = new Api();

            ViewBag.Genero = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("Persona/GetGeneros"), Constantes.Select);
            ViewBag.Roles = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("Persona/GetRoles"), Constantes.Select);
            ViewBag.TipoDocumento = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("Persona/GetTipoDocumentos"), Constantes.Select);
            ViewBag.Empresas = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("Empresa/GetEmpresas"), Constantes.Select);
            if (id.HasValue)
            {
                ViewBag.EditUser = API.Get<Models.Acceso.Usuario>("Usuario/GetUsuario", new Dictionary<string, string> { { "id", id.Value.ToString() } });
                ViewBag.EditPerson = API.Get<Models.Comun.Persona>("Persona/GetPersona", new Dictionary<string, string> { { "id", ViewBag.EditUser.PersonaId.ToString() } });
            }

            return View();
        }

        [GeneralSecurity(Rol = "Seguridad-Usuario de Sistema")]
        public ActionResult GetAccordion(string data)
        {
            ViewBag.Accordion = data;
            return PartialView("_AccordionPartial");
        }

        [GeneralSecurity(Rol = "Seguridad-Usuario de Sistema")]
        public JsonResult GetTreeData(int data)
        {
            Api API = new Api();
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("id", data.ToString());
            List<TreeView> Tree = API.Get<List<TreeView>>("Usuario/GetTreeView", args);
            return Json(Tree);
        }

        [GeneralSecurity(Rol = "Seguridad-Usuario de Sistema")]
        public JsonResult AddNewRol(string input, string tree)
        {
            Api API = new Api();
            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "String1", input },
                { "String2", tree},
                { "Int1", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            Parametro response = API.Post<Parametro>("Usuario/InsertRol", args);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Seguridad-Usuario de Sistema")]
        public JsonResult InsertNewPerson(string Persona, string Usuario)
        {
            Api API = new Api();
            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "String1", Persona },
                { "String2", Usuario },
                { "Int1", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Post<bool>("Persona/InsertNewPersona", args);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Seguridad-Usuario de Sistema")]
        public JsonResult EditPerson(string Persona, string Usuario)
        {
            Api API = new Api();
            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "String1", Persona },
                { "String2", Usuario },
                { "Int1", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Post<bool>("Persona/EditPersona", args);
            return Json(response);
        }
    }
}