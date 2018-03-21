using OTracking.Models;
using OTracking.Models.Comun;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OTracking.Controllers.Seguimiento
{
    public class SeguimientoController : Controller
    {
        public ActionResult GenerarPlantilla()
        {
            Api API = new Api();

            Dictionary<string, string> args = new Dictionary<string, string>()
            {
                {"","" }
            };

            ViewBag.Bandeja = API.Post<BandejaGenerarPlantilla>("Seguimiento/ObtenerBandejaGenerarPlantilla", args);
            return View();
        }
    }
}