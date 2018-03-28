using OTracking.Controllers.Seguridad;
using System.Web.Mvc;

namespace OTracking.Controllers.Seguimiento
{
    public class SeguimientoController : Controller
    {
        [GeneralSecurity(Rol = "Seguimiento-Seguimiento")]
        public ActionResult Calendario()
        {
            return View();
        }
    }
}