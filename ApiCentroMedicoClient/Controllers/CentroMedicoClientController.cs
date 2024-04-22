using ApiCentroMedicoClient.Filters;
using ApiCentroMedicoClient.Models;
using ApiCentroMedicoClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiCentroMedicoClient.Controllers
{
    public class CentroMedicoClientController : Controller
    {
        private ServiceApiCentroMedicoClient service;
        public CentroMedicoClientController(ServiceApiCentroMedicoClient service)
        {
            this.service = service;
        }

        //Controller , nos muestra una pagina segun el rol.
        [AuthorizeUsers]
        public async Task<IActionResult> AdministradorPrincipal()
        {
            ViewData["NOMBREUSUARIO"] = HttpContext.User.Identity.Name ;
            ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }
        [AuthorizeUsers]
        public async Task<IActionResult> RecepcionistaPrincipal()
        {
            ViewData["NOMBREUSUARIO"] = HttpContext.User.Identity.Name;
            ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }
        [AuthorizeUsers]
        public async Task<IActionResult> MedicoPrincipal()
        {
            ViewData["NOMBREUSUARIO"] = HttpContext.User.Identity.Name;
            ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }
        [AuthorizeUsers]
        public async Task<IActionResult> PacientePrincipal()
        {
            ViewData["NOMBREUSUARIO"] = HttpContext.User.Identity.Name;
            ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }

        // ============ Controller Recepcionista ============ //
        public async Task<IActionResult> RecepcionistaFindPacienteCita()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RecepcionistaFindPacienteCita(string nombre, string apellido , string correo)
        {
            Paciente paciente = await this.service.FindPacienteCitaRecepcionista(nombre,apellido,correo);
            return View(paciente);
        }

    }
}
