using ApiCentroMedicoClient.Filters;
using ApiCentroMedicoClient.Models;
using ApiCentroMedicoClient.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorPrincipal()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Usuario user = await this.service.FindUsuario(id);
            ViewData["NOMBREUSUARIO"] = user.Nombre;
            ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }
        [AuthorizeUsers(Policy = "SOLORECEPCIONISTA")]
        public async Task<IActionResult> RecepcionistaPrincipal()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Usuario user = await this.service.FindUsuario(id);
            ViewData["NOMBREUSUARIO"] = user.Nombre;
            ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }
        [AuthorizeUsers(Policy = "SOLOMEDICO")]
        public async Task<IActionResult> MedicoPrincipal()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Usuario user = await this.service.FindUsuario(id);
            ViewData["NOMBREUSUARIO"] = user.Nombre;
            ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }
        [AuthorizeUsers(Policy = "SOLOPACIENTE")]
        public async Task<IActionResult> PacientePrincipal()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Usuario user = await this.service.FindUsuario(id);
            ViewData["NOMBREUSUARIO"] = user.Nombre;
            ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }



        // ============ Controller Administrador ============ //
        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorPerfil()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Usuario user = await this.service.FindUsuario(id);
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> AdministradorPerfil(Usuario user)
        {
            await this.service.PutUsuario(user.Id,user.Nombre,user.Apellido,user.Correo,user.Contra,user.Id_EstadoUsuario,user.Id_TipoUsuario);
            return RedirectToAction("AdministradorPrincipal");
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorListaUsuarios()
        {
            ViewData["TIPOUSUARIO"] = await this.service.GetTipoUsuario();
            List<Usuario> usuarios = await this.service.GetUsuariosAsync();
            return View(usuarios);
        }
        [HttpPost]
        public async Task<IActionResult> AdministradorListaUsuarios(int tipoUsuario)
        {
            ViewData["TIPOUSUARIO"] = await this.service.GetTipoUsuario();
            List<Usuario> usuarios = await this.service.GetUsuariosTipoAsync(tipoUsuario);
            return View(usuarios);
        }




        // ============ Controller Recepcionista ============ //
        [AuthorizeUsers(Policy = "SOLORECEPCIONISTA")]
        public async Task<IActionResult> RecepcionistaPerfil()
        {
            int idrecepcionista = int.Parse(this.HttpContext.User.FindFirst("ID").Value); 
            Usuario recepcionista = await this.service.FindUsuario(idrecepcionista);
            return View(recepcionista);
        }
        [HttpPost]
        public async Task<IActionResult> RecepcionistaPerfil(Usuario user)
        {
            this.service.PutUsuario(user.Id, user.Nombre, user.Apellido, user.Correo, user.Contra, user.Id_EstadoUsuario, user.Id_TipoUsuario);
            return RedirectToAction("RecepcionistaPrincipal");
        }

        [AuthorizeUsers(Policy = "SOLORECEPCIONISTA")]
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
