using OTracking.Models.Comun;
using OTracking.Controllers.Seguridad;
using System.Collections.Generic;
using System.Web.Mvc;
using OTracking.Models;
using Newtonsoft.Json;
using System;

namespace OTracking.Controllers.Seguimiento
{
    public class SeguimientoController : Controller
    {
        [GeneralSecurity(Rol = "Seguimiento-Seguimiento")]
        public ActionResult Calendario()
        {
            ViewBag.Seguimiento = new BandejaSeguimiento() { Lista = new List<BandejaSeguimientoDetalle>(), Take = 10 };
            return View();
        }

        [GeneralSecurity(Rol = "Seguimiento-Seguimiento")]
        public ActionResult Filtrar(BandejaSeguimiento data)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "Nombre",data.Nombre },
                { "NroDocumento", data.NroDocumento },
                { "Index", data.Index.ToString()},
                { "Take", data.Take.ToString()}
            };
            ViewBag.Seguimiento = API.Post<BandejaSeguimiento>("Seguimiento/ObtenerListadoSeguimiento", arg);
            return PartialView("_BandejaCronogramaPartial");
        }

        [GeneralSecurity(Rol = "Seguimiento-Seguimiento")]
        public JsonResult ConfirmarSeguimiento(string id)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "seguimientoId",id },
                { "UserId", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Get<bool>("Seguimiento/ConfirmarSeguimiento", arg);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Seguimiento-Seguimiento")]
        public JsonResult EliminarSeguimiento(string id)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "seguimientoId",id },
                { "UserId", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Get<bool>("Seguimiento/EliminarSeguimiento", arg);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Seguimiento-Seguimiento")]
        public JsonResult NotificarSeguimiento(List<int> SeguimientoIdList)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "json", JsonConvert.SerializeObject(SeguimientoIdList) }
            };
            string response = API.Get<string>("Seguimiento/NotificarSeguimiento", arg);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Seguimiento-Seguimiento")]
        public JsonResult CambiarSeguimiento(string id, string fecha)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "seguimientoId",id },
                { "fechaString", fecha },
                { "UserId", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Get<bool>("Seguimiento/CambiarSeguimiento", arg);
            return Json(response);
        }
    }
}