using OTracking.Models;
using OTracking.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OTracking.Controllers
{
    public class GeneralsController : Controller
    {
        // GET: Generals
        public ActionResult Index()
        {
            return RedirectToRoute("General_login");
        }

        public ActionResult Login()
        {
            if (TempData["MESSAGE"] != null)
            {
                ViewBag.MESSAGE = TempData["MESSAGE"];
            }
            return View("~/Views/Generals/Login.cshtml");
        }

        public ActionResult Login_authentication(FormCollection collection)
        {
            if (TempData["FormCollection"] != null)
                collection = (FormCollection)TempData["FormCollection"];

            if (ValidarCamposVacios(collection))
                ValidarAccesoUsuario(collection);           
            else
                TempData["MESSAGE"] = "Debe ingresar el usuario y/o la contraseña";
        
            return RedirectToRoute("General_Login");
        }

        private void ValidarAccesoUsuario(FormCollection collection)
        {
            TempData["FormCollection"] = null;

            Api API = new Api();
            ViewBag.USUARIO = API.Get<Models.Acceso.UsuarioLogin>(relativePath: "Usuario/GetUsuario", args: Argumentos(collection));
            if (ViewBag.USUARIO != null)
            {
                Session.Add("AutBackoffice", PoblarClientSession(ViewBag.USUARIO));
                RedirectToRoute("backoffice");
            }
            else
            {
                TempData["MESSAGE"] = "Usuario o contraseña incorrectos";
                RedirectToRoute("General_Login");
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
        
    }
}