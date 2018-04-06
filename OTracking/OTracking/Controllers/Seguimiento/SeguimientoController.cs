using OTracking.Models.Comun;
using OTracking.Controllers.Seguridad;
using System.Collections.Generic;
using System.Web.Mvc;
using OTracking.Models;
using Newtonsoft.Json;
using System;
using OTracking.Utils;

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
        public JsonResult ConfirmarSeguimiento(int id)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "seguimientoId",id.ToString() },
                { "UserId", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Get<bool>("Seguimiento/ConfirmarSeguimiento", arg);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Seguimiento-Seguimiento")]
        public JsonResult EliminarSeguimiento(int id)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "seguimientoId",id.ToString() },
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
        public JsonResult CambiarSeguimiento(int id, string fecha)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "seguimientoId",id.ToString() },
                { "fechaString", fecha },
                { "UserId", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Get<bool>("Seguimiento/CambiarSeguimiento", arg);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Seguimiento-Seguimiento")]
        public JsonResult AdjuntarCita(int id, string archivo, string nombre)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "String1", archivo },
                { "String2", nombre },
                { "Int1", id.ToString() },
                { "Int2", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Post<bool>("Seguimiento/AdjuntarCita", arg);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Seguimiento-Seguimiento")]
        public JsonResult IngresarControl(int id, decimal input)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "seguimientoId", id.ToString() },
                { "valorControl", input.ToString() },
                { "UserId", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Get<bool>("Seguimiento/IngresarControl", arg);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Seguimiento-Servicios")]
        public ActionResult Servicios()
        {
            Api API = new Api();
            ViewBag.Servicios = new BandejaServicios() { Lista = new List<BandejaServiciosDetalle>(), Take = 10};
            ViewBag.TipoSeguimeinto = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("Seguimiento/ObtenerListadoTipoSeguimiento"), Constantes.Select);
            ViewBag.TipoControl = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("PlanDeVida/ObtenerTipoControl"), Constantes.Select);
            return View();
        }

        [GeneralSecurity(Rol = "Seguimiento-Servicios")]
        public ActionResult FiltrarServicios(BandejaServicios data)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "Nombre",data.Nombre },
                { "NroDocumento", data.NroDocumento},
                { "Index", data.Index.ToString()},
                { "Take", data.Take.ToString()}
            };
            ViewBag.Servicios = API.Post<BandejaServicios>("Seguimiento/FiltrarServicios", arg);
            return PartialView("_BandejaServiciosPartial");
        }
    }
}