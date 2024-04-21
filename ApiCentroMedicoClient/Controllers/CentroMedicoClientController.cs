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
        [AuthorizeUsers]
        public async Task<IActionResult> Index()
        {
            List<Usuario> usuarios = await this.service.GetUsuariosAsync();
            return View(usuarios);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Details(int id)
        {
            Usuario user = await this.service.FindUsuario(id);
            return View(user);
        }
    }
}
