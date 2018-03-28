using OTracking.Models.Comun;
using OTracking.Controllers.Seguridad;
using System.Collections.Generic;
using System.Web.Mvc;
using OTracking.Models;

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
                { "Index", data.Index.ToString()},
                { "Take", data.Take.ToString()}
            };
            ViewBag.Seguimiento = API.Post<BandejaSeguimiento>("Seguimiento/ObtenerListadoSeguimiento", arg);
            return PartialView("_BandejaCronogramaPartial");
        }
    }
}