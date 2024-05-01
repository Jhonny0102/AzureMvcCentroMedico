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
            Usuario user = await this.service.FindUsuarioAsync(id);
            ViewData["NOMBREUSUARIO"] = user.Nombre;
            //ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }
        [AuthorizeUsers(Policy = "SOLORECEPCIONISTA")]
        public async Task<IActionResult> RecepcionistaPrincipal()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Usuario user = await this.service.FindUsuarioAsync(id);
            ViewData["NOMBREUSUARIO"] = user.Nombre;
            //ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }
        [AuthorizeUsers(Policy = "SOLOMEDICO")]
        public async Task<IActionResult> MedicoPrincipal()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Usuario user = await this.service.FindUsuarioAsync(id);
            ViewData["NOMBREUSUARIO"] = user.Nombre;
            //ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }
        [AuthorizeUsers(Policy = "SOLOPACIENTE")]
        public async Task<IActionResult> PacientePrincipal()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Usuario user = await this.service.FindUsuarioAsync(id);
            ViewData["NOMBREUSUARIO"] = user.Nombre;
            //ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            return View();
        }



        // ============ Controller Administrador ============ //

        //Controller donde recogemos la informacion del administrador y guardamos la nueva. 
        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorPerfil()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Usuario user = await this.service.FindUsuarioAsync(id);
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> AdministradorPerfil(Usuario user)
        {
            await this.service.PutUsuarioAsync(user.Id,user.Nombre,user.Apellido,user.Correo,user.Contra,user.Id_EstadoUsuario,user.Id_TipoUsuario);
            return RedirectToAction("AdministradorPrincipal");
        }

        //Controller donde mostramos y devolvemos un conjunto de usuarios ,segun el filtro.
        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorListaUsuarios()
        {
            ViewData["TIPOUSUARIO"] = await this.service.GetTipoUsuario();
            ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            List<Usuario> usuarios = await this.service.GetUsuariosAsync();
            return View(usuarios);
        }
        [HttpPost]
        public async Task<IActionResult> AdministradorListaUsuarios(int tipoUsuario)
        {
            ViewData["TIPOUSUARIO"] = await this.service.GetTipoUsuario();
            ViewData["IDUSUARIO"] = HttpContext.User.FindFirst(z => z.Type == "ID").Value;
            List<Usuario> usuarios = await this.service.GetUsuariosTipoAsync(tipoUsuario);
            return View(usuarios);
        }

        //Controller donde recogemos 2 parametros, el idusuario y el idtipo.
        //Segun el tipo le redirecciona a otro controller. Esto es necesario porque a la hora de llamar al controller, desde la vista ,
        //no podemos indicarle a cada uno el controller que debe de usar asi que le enviamos uno (este) que redirige automaticamente.

        //Informacion(Details)
        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public IActionResult AdministradorInformacion(int idusuario, int idtipo)
        {
            TempData["IDUSUARIOINFO"] = idusuario;
            if (idtipo == 1 || idtipo == 2)
            {
                return RedirectToAction("AdministradorInfoUsuario");
            }
            else if (idtipo == 3)
            {
                return RedirectToAction("AdministradorInfoMedico");
            }
            else
            {
                return RedirectToAction("AdministradorInfoPaciente");
            }
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorInfoUsuario()
        {
            int idusuario = (int)TempData["IDUSUARIOINFO"];
            UsuarioDetallado usuario = await this.service.GetUsuarioDetallado(idusuario);
            return View(usuario);
            
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorInfoMedico()
        {
            int idmedico = (int)TempData["IDUSUARIOINFO"];
            MedicoDetallado medico = await this.service.GetMedicoDetallado(idmedico);
            return View(medico);
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorInfoPaciente()
        {
            int idpaciente = (int)TempData["IDUSUARIOINFO"];
            PacienteDetallado paciente = await this.service.GetPacienteDetallado(idpaciente);
            return View(paciente);
        }

        //Actualizar(Update)
        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public IActionResult AdministradorActualizar(int idusuario, int idtipo)
        {
            TempData["IDUSUARIOACTUALIZAR"] = idusuario;
            if (idtipo == 1 || idtipo == 2)
            {
                return RedirectToAction("AdministradorActualizarUsuario");
            }
            else if (idtipo == 3)
            {
                return RedirectToAction("AdministradorActualizarMedico");
            }
            else
            {
                return RedirectToAction("AdministradorActualizarPaciente");
            }
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorActualizarUsuario()
        {
            int idusuario = (int)TempData["IDUSUARIOACTUALIZAR"];
            ViewData["ESTADOS"] = await this.service.GetEstadosAsync();
            Usuario usuario = await this.service.FindUsuarioAsync(idusuario);
            return View(usuario);
        }
        [HttpPost]
        public async Task<IActionResult> AdministradorActualizarUsuario(Usuario user)
        {
            await this.service.PutUsuarioAsync(user.Id, user.Nombre, user.Apellido, user.Correo, user.Contra, user.Id_EstadoUsuario, user.Id_TipoUsuario);
            return RedirectToAction("AdministradorListaUsuarios"); //La vista no redirige al usar el seet alert
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorActualizarMedico()
        {
            int idmedico = (int)TempData["IDUSUARIOACTUALIZAR"];
            ViewData["ESTADOS"] = await this.service.GetEstadosAsync();
            ViewData["ESPECIALIDADES"] = await this.service.GetEspecialidadesAsync();
            Medico medico = await this.service.FindMedicoAsync(idmedico);
            return View(medico);
        }
        [HttpPost]
        public async Task<IActionResult> AdministradorActualizarMedico(Medico medico)
        {
            await this.service.UpdateMedicoAsync(medico.Id,medico.Nombre,medico.Apellido,medico.Correo,medico.Contra,medico.EstadoUsuario,medico.TipoUsuario,medico.Especialidad);
            return RedirectToAction("AdministradorListaUsuarios"); //La vista no redirige al usar el seet alert
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorActualizarPaciente()
        {
            int idpaciente = (int)TempData["IDUSUARIOACTUALIZAR"];
            ViewData["ESTADOS"] = await this.service.GetEstadosAsync();
            Paciente paciente = await this.service.FindPacienteAsync(idpaciente);
            return View(paciente);
        }
        [HttpPost]
        public async Task<IActionResult> AdministradorActualizarPaciente(Paciente pac)
        {
            await this.service.UpdatePacienteAsync(pac.Id,pac.Nombre,pac.Apellido,pac.Correo,pac.Contra,pac.Telefono,pac.Direccion,pac.Edad,pac.Genero,pac.EstadoUsuario,pac.TipoUsuario);
            return RedirectToAction("AdministradorListaUsuarios"); //La vista no redirige al usar el seet alert
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorEliminarUsuario(int idusuario, int idtipo)
        {
            await this.service.DeleteUsuarioAsync(idusuario,idtipo);
            return RedirectToAction("AdministradorListaUsuarios");
        } 





        // ============ Controller Recepcionista ============ //
        [AuthorizeUsers(Policy = "SOLORECEPCIONISTA")]
        public async Task<IActionResult> RecepcionistaPerfil()
        {
            int idrecepcionista = int.Parse(this.HttpContext.User.FindFirst("ID").Value); 
            Usuario recepcionista = await this.service.FindUsuarioAsync(idrecepcionista);
            return View(recepcionista);
        }
        [HttpPost]
        public async Task<IActionResult> RecepcionistaPerfil(Usuario user)
        {
            this.service.PutUsuarioAsync(user.Id, user.Nombre, user.Apellido, user.Correo, user.Contra, user.Id_EstadoUsuario, user.Id_TipoUsuario);
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
