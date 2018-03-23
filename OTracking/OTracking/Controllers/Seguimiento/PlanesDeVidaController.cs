using OTracking.Models;
using OTracking.Models.Comun;
using OTracking.Utils;
using System.Collections.Generic;
using OTracking.Controllers.Seguridad;
using System.Web.Mvc;

namespace OTracking.Controllers.Seguimiento
{
    public class PlanesDeVidaController : Controller
    {
        [GeneralSecurity(Rol = "Seguimiento-Planes De Vida")]
        public ActionResult BandejaPlanesDeVida()
        {
            return View();
        }

        [GeneralSecurity(Rol = "Seguimiento-Planes De Vida")]
        public ActionResult CrearPlanDeVida(int? id = null)
        {
            Api API = new Api();
            ViewBag.TipoTiempo = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("PlanDeVida/ObtenerTipoTiempo"),Constantes.Select);
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
    }
}