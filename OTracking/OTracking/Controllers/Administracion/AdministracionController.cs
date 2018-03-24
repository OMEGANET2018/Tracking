using OTracking.Models;
using OTracking.Models.Comun;
using System.Collections.Generic;
using OTracking.Controllers.Seguridad;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using OTracking.Utils;

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
            ViewBag.Componentes = API.Get<List<BandejaComponente>>("Componente/ObtenerListadoComponentes");
            ViewBag.TipoValores = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("Componente/ObtenerListaTipoValorComponentes"),Constantes.Select);
            return View();
        }

        [GeneralSecurity(Rol = "Administracion-Configurador de Componentes")]
        public ActionResult ObtenerListadoComponentes()
        {
            Api API = new Api();

            ViewBag.Componentes = API.Get<List<BandejaComponente>>("Componente/ObtenerListadoComponentes");

            return PartialView("_BandejaComponentesPartial");
        }

        [GeneralSecurity(Rol = "Administracion-Configurador de Componentes")]
        public JsonResult EnviarComponente(BandejaComponente data)
        {
            Api API = new Api();

            data.UsuGraba = ViewBag.USUARIO.UsuarioId;

            Dictionary<string, string> args = new Dictionary<string, string>()
            {
                { "String1", JsonConvert.SerializeObject(data) }
            };

            bool saved = API.Post<bool>("Componente/GuardarCambiosComponente", args);

            if (saved)
                return Json(saved);
            else
                return Json(null);
        }

        [GeneralSecurity(Rol = "Administracion-Proveedores")]
        public ActionResult Proveedores()
        {
            ViewBag.Proveedores = new BandejaProveedores() { Lista = new List<BandejaProveedoresLista>(), Take = 10 };
            return View();
        }

        [GeneralSecurity(Rol = "Administracion-Proveedores")]
        public ActionResult CrearProveedor(int? id)
        {
            Api API = new Api();

            ViewBag.TipoEmpresa = Utils.Utils.LoadDropDownList(API.Get<List<Dropdownlist>>("Empresa/GetTipoEmpresa"), Constantes.Select);
            if (id.HasValue)
            {
                ViewBag.Empresa = API.Get<BandejaProveedoresLista>("Empresa/GetEmpresaPorId", new Dictionary<string, string> { { "id", id.Value.ToString() } });
            }

            return View();
        }

        [GeneralSecurity(Rol = "Administracion-Proveedores")]
        public ActionResult FiltrarProveedor(BandejaProveedores data)
        {
            Api API = new Api();
            Dictionary<string, string> arg = new Dictionary<string, string>()
            {
                { "RazonSocial",data.RazonSocial },
                { "RUC", data.RUC},
                { "Index", data.Index.ToString()},
                { "Take", data.Take.ToString()}
            };
            ViewBag.Proveedores = API.Post<BandejaProveedores>("Empresa/FiltrarEmpresa", arg);
            return PartialView("_BandejaProveedoresPartial");
        }

        [GeneralSecurity(Rol = "Administracion-Proveedores")]
        public JsonResult DeleteProveedor(int id)
        {
            Api API = new Api();
            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "Int1", id.ToString() },
                { "Int2", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Post<bool>("Empresa/DeleteEmpresa", args);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Administracion-Proveedores")]
        public JsonResult EditProveedor(BandejaProveedoresLista data)
        {
            Api API = new Api();
            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "String1", JsonConvert.SerializeObject(data) },
                { "Int1", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Post<bool>("Empresa/EditEmpresa", args);
            return Json(response);
        }

        [GeneralSecurity(Rol = "Administracion-Proveedores")]
        public JsonResult InsertNewProveedor(BandejaProveedoresLista proveedor)
        {
            Api API = new Api();
            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "String1", JsonConvert.SerializeObject(proveedor) },
                { "Int1", ViewBag.USUARIO.UsuarioId.ToString() }
            };
            bool response = API.Post<bool>("Empresa/InsertNewEmpresa", args);
            return Json(response);
        }
    }
}