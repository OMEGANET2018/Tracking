using OTracking.Controllers.Seguridad;
using OTracking.Models;
using OTracking.Models.Acceso;
using OTracking.Utils;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OTracking.Controllers
{
    public class GeneralsController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToRoute("General_login");
        }

        [GeneralSecurity(Rol = "")]
        public ActionResult Home()
        {
            return View("~/Views/Generals/Index.cshtml", ViewBag.MENU);
        }

        public ActionResult Login()
        {
            if (TempData["MESSAGE"] != null)
            {
                ViewBag.MESSAGE = TempData["MESSAGE"];
            }
            return View("~/Views/Generals/Login.cshtml");
        }

        public ActionResult Logout()
        {
            Session.Remove("AutBackoffice");
            Session.RemoveAll();
            return RedirectToRoute("General_login");
        }

        public ActionResult Login_authentication(FormCollection collection)
        {
            if (TempData["FormCollection"] != null)
                collection = (FormCollection)TempData["FormCollection"];

            if (ValidarCamposVacios(collection))
            {
                if (ValidarAccesoUsuario(collection))
                    return RedirectToRoute("tracking");
                else
                    return RedirectToRoute("General_Login");
            }               
            else
            {
                TempData["MESSAGE"] = "Debe ingresar el usuario y/o la contraseña";
                return RedirectToRoute("General_Login");
            }
                
        }

        private bool ValidarAccesoUsuario(FormCollection collection)
        {
            TempData["FormCollection"] = null;

            Api API = new Api();
            var usuarioLogeado = API.Get<UsuarioLogin>(relativePath: "Usuario/GetUsuario", args: Argumentos(collection));
            if (usuarioLogeado != null)
            {
                Session.Add("AutBackoffice", PoblarClientSession(usuarioLogeado));
                return true;
            }
            else
            {
                TempData["MESSAGE"] = "Usuario o contraseña incorrectos";
                return false;
            }
        }        

        private bool ValidarCamposVacios(FormCollection collection)
        {
            if (string.IsNullOrWhiteSpace(collection.Get("usuario").Trim()) || string.IsNullOrWhiteSpace(collection.Get("pass").Trim()))
                return false;

            return true;
        }

        private ClientSession PoblarClientSession(dynamic usuario)
        {
            ViewBag.USUARIO = usuario;
            ClientSession oclientSession = new ClientSession
            {
                UsuarioId = ViewBag.USUARIO.UsuarioId,
                PersonaId = ViewBag.USUARIO.PersonaId,
                EmpresaId = ViewBag.USUARIO.EmpresaId,
                foto = ViewBag.USUARIO.foto,
                NombreUsuario = ViewBag.USUARIO.NombreUsuario,
                NombreCompleto = ViewBag.USUARIO.NombreCompleto,
                FechaCaduca = ViewBag.USUARIO.FechaCaduca,
                RolId = ViewBag.USUARIO.RolId,
                Rol = ViewBag.USUARIO.Rol,
                Autorizacion = ViewBag.USUARIO.Autorizacion
            };
            return oclientSession;
        }

        private Dictionary<string, string> Argumentos(FormCollection collection)
        {
            Dictionary<string, string> accesoUsuario = new Dictionary<string, string>
            {
                { "usuario", collection.Get("usuario").Trim() },
                { "contrasenia", Utils.Utils.Encrypt(collection.Get("pass").Trim()) }
            };

            return accesoUsuario;
        }

        public ActionResult SessionExpired()
        {
            Session.Remove("AutBackoffice");
            Session.RemoveAll();
            return RedirectToRoute("General_login");
        }
    }
}