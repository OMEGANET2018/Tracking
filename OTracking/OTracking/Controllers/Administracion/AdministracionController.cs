using OTracking.Models;
using OTracking.Models.Comun;
using System.Collections.Generic;
using OTracking.Controllers.Seguridad;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace OTracking.Controllers.Administracion
{
    public class AdministracionController : Controller
    {
        [GeneralSecurity(Rol = "Administracion-Carga Planilla")]
        public ActionResult CargaPlanilla()
        {
            return View();
        }

        [GeneralSecurity(Rol = "Administracion-Carga Planilla")]
        public JsonResult InsertarPersonaExcel()
        {
            Api API = new Api();

            byte[] arr = null;

            using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
            {
                arr = binaryReader.ReadBytes(Request.Files[0].ContentLength);
            }

            string response = JsonConvert.DeserializeObject<string>(API.PostUploadStream("Persona/InsertarPersonaExcel", arr));

            return Json(response);
        }

        [GeneralSecurity(Rol = "Administracion-Configurador de Componentes")]
        public ActionResult ConfiguradorComponentes()
        {
            Api API = new Api();
            ViewBag.Componentes = new List<BandejaComponente>();
            ViewBag.TipoValores = API.Get<List<Dropdownlist>>("Componente/ObtenerListaTipoValorComponentes");
            return View();
        }

        [GeneralSecurity(Rol = "Administracion-Configurador de Componentes")]
        public ActionResult ObtenerListadoComponentes()
        {
            Api API = new Api();

            ViewBag.Componentes = API.Get<List<BandejaComponente>>("Administracion/ObtenerListadoComponente");

            return PartialView("_BandejaComponentesPartial");
        }
    }
}