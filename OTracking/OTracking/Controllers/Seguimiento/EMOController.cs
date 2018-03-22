using OTracking.Utils;
using OTracking.Controllers.Seguridad;
using OTracking.Models;
using OTracking.Models.Comun;
using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace OTracking.Controllers.Seguimiento
{
    public class EMOController : Controller
    {
        [GeneralSecurity(Rol = "Seguimiento-EMO")]
        public ActionResult BandejaEMO()
        {
            Api API = new Api();
            ViewBag.Bandeja = new BandejaEMO() { Lista = new List<BandejaEMODetalle>(), Take = 10};
            ViewBag.Empresas = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("Empresa/GetEmpresas"),Constantes.Select);
            return View();
        }

        [GeneralSecurity(Rol = "Seguimiento-EMO")]
        public ActionResult FiltrarBandejaEMO(BandejaEMO data)
        {
            Api API = new Api();

            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "Nombre", data.Nombre },
                { "Index", data.Index.ToString()},
                { "Take", data.Take.ToString()}
            };

            ViewBag.Bandeja = API.Post<BandejaEMO>("EMO/ObtenerBandejaEMO", arg);
            return PartialView("_BandejaEMOPartial");
        }

        [GeneralSecurity(Rol = "Seguimiento-EMO")]
        public JsonResult GenerarEMO(BandejaEMO data)
        {
            Api API = new Api();

            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "String1", JsonConvert.SerializeObject(data)}
            };

            byte[] ms = API.PostDownloadStream("EMO/CrearEMO", arg);

            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=Probando.xlsx");
            Response.BinaryWrite(ms);
            Response.End();

            return Json(Response);
        }
    }
}