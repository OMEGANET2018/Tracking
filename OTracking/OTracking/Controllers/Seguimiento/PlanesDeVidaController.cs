using OTracking.Models;
using OTracking.Models.Comun;
using OTracking.Utils;
using System.Collections.Generic;
using OTracking.Controllers.Seguridad;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace OTracking.Controllers.Seguimiento
{
    public class PlanesDeVidaController : Controller
    {
        [GeneralSecurity(Rol = "Seguimiento-Planes De Vida")]
        public ActionResult BandejaPlanesDeVida()
        {
            Api API = new Api();
            Dictionary<string, string> args = new Dictionary<string, string>()
            {
                { "Take","3" },
                { "Index", "1" }
            };

            ViewBag.PlanesDeVida = API.Post<BandejaPlanDeVida>("PlanDeVida/ObtenerListadoPlanesDeVida",args);
            return View();
        }

        [GeneralSecurity(Rol = "Seguimiento-Planes De Vida")]
        public ActionResult CrearPlanDeVida(int? id = null)
        {
            Api API = new Api();
            ViewBag.TipoTiempo = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("PlanDeVida/ObtenerTipoTiempo"),Constantes.Select);
            ViewBag.TipoControl = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("PlanDeVida/ObtenerTipoControl"), Constantes.Select);

            Dictionary<string, string> arg = new Dictionary<string, string>();

            if (id.HasValue)
            {
                arg.Add("id", id.Value.ToString());
                ViewBag.PlanDeVida = API.Get<PlanesDeVida>("PlanDeVida/ObtenerPlanDeVidaPorId",arg);
            }

            return View();
        }

        [GeneralSecurity(Rol = "Seguimiento-Planes De Vida")]
        public JsonResult ObtenerDxConCIE10(string data)
        {
            Api API = new Api();

            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "data", data }
            };

            string response = API.Get<string>("PlanDeVida/ObtenerDxConCIE10",arg);

            return Json(response);
        }

        [GeneralSecurity(Rol = "Seguimiento-Planes De Vida")]
        public JsonResult EnviarPlanDeVida(PlanesDeVida data)
        {
            data.UsuGraba = ViewBag.USUARIO.UsuarioId;

            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "String1", JsonConvert.SerializeObject(data) }
            };

            bool saved = API.Post<bool>("PlanDeVida/GuardarCambiosPlanDeVida", arg);

            if (saved)
                return Json(saved);
            else
                return Json(null);
        }

        [GeneralSecurity(Rol = "Seguimiento-Planes De Vida")]
        public ActionResult ObtenerListadoPlanesDeVida(BandejaPlanDeVida Data)
        {
            Api API = new Api();
            Dictionary<string, string> args = new Dictionary<string, string>()
            {
                { "Take", Data.Take.ToString() },
                { "Index", Data.Index.ToString() }
            };
            ViewBag.PlanesDeVida = API.Post<BandejaPlanDeVida>("PlanDeVida/ObtenerListadoPlanesDeVida",args);
            return PartialView("_PlanesDeVidaPartial");
        }
    }
}