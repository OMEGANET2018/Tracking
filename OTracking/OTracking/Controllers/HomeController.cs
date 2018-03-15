using OTracking.Models;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace OTracking.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

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
    }
}