using ApiCentroMedicoClient.Filters;
using ApiCentroMedicoClient.Models;
using ApiCentroMedicoClient.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        //Controller donde manejamos los usuarios de la BBDD.
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

        //Controller que permite crear recepcionista/administrador y medicos.
        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorCreateMedico()
        {
            ViewData["ESPECIALIDADES"] = await this.service.GetEspecialidadesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdministradorCreateMedico(Medico medico)
        {
            await this.service.CreateMedicoAsync(medico.Nombre, medico.Apellido, medico.Correo, medico.Contra, medico.Especialidad);
            return RedirectToAction("AdministradorListaUsuarios");
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorCreateUsuario()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AdministradorCreateUsuario(Usuario user)
        {
            await this.service.CreateUsuarioAsync(user.Nombre,user.Apellido,user.Correo,user.Contra,user.Id_TipoUsuario);
            return RedirectToAction("AdministradorListaUsuarios");
        }

        //Controller de manejo de citas.
        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorGetListaCitas()
        {
            List<CitaDetallado> citas = await this.service.GetCitasAsync();
            return View(citas);
        }
        [HttpPost]
        public async Task<IActionResult> AdministradorGetListaCitas(DateTime fecha , DateTime? fechahasta)
        {
            List<CitaDetallado> citas;
            if (fechahasta != null)
            {
                citas = await this.service.GetCitasFiltroAsync(fecha,fechahasta.Value);
            }
            else
            {
                citas = await this.service.GetCitasFiltroAsync(fecha,null);
            }
            
            return View(citas);
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorFindCita(int idcita)
        {
            Cita cita = await this.service.FindCitaAsync(idcita);
            Usuario paciente = await this.service.FindUsuarioAsync(cita.Paciente);
            Usuario medico = await this.service.FindUsuarioAsync(cita.Medico);
            SeguimientoCita seguimientocita = await this.service.FindSeguimientoCitaAsync(cita.SeguimientoCita);
            ViewData["NOMBREMEDICO"] = medico.Nombre + " " + medico.Apellido ;
            ViewData["NOMBREPACIENTE"] = paciente.Nombre + " " + paciente.Apellido;
            ViewData["SEGUIMIENTO"] = seguimientocita.Estado;
            return View(cita);
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorFindMedicoCita(int idmedico, int idcita)
        {
            MedicoDetallado medico = await this.service.GetMedicoDetallado(idmedico);
            ViewData["CITASELECCIONADA"] = idcita;
            return View(medico);
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorFindPacienteCita(int idpaciente, int idcita)
        {
            PacienteDetallado paciente = await this.service.GetPacienteDetallado(idpaciente);
            ViewData["CITASELECCIONADA"] = idcita;
            return View(paciente);
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorDeleteCita(int idcita)
        {
            await this.service.DeleteCitaAsync(idcita);
            return RedirectToAction("AdministradorGetListaCitas");
        }

        //Controller para el manejo de peticiones de usuarios
        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorListaPeticionesUsuarios()
        {
            List<PeticionesDetallado> peticionesUsuarios = await this.service.GetPeticionesUsuarioAsync();
            return View(peticionesUsuarios);
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorAceptarPeticionUsuario(int idpeticion, int idusuario, int estado)
        {
            await this.service.AceptPeticionUsuarioAsync(idpeticion,idusuario,estado);
            return RedirectToAction("AdministradorListaPeticionesUsuarios");
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorDenegarPeticionUsuario(int idpeticion)
        {
            await this.service.DeletePeticionUsuarioAsync(idpeticion);
            return RedirectToAction("AdministradorListaPeticionesUsuarios");
        }

        //Controller para manejo de peticiones medicamentos.
        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorListaPeticionesMedicamentos()
        {
            List<PeticionesMedicamentoDetallado> peticionesMedicamentos = await this.service.GetPeticionesMedicamentosAsync();
            return View(peticionesMedicamentos);
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorAceptarMedicamento(int idpeticion, int? idmedicamento, string nombre, string descripcion, int estado)
        {
            if (idmedicamento != null)
            {
                await this.service.AceptPeticionMedicamentoUpdate(idpeticion,idmedicamento.Value,estado);
                return RedirectToAction("AdministradorListaPeticionesMedicamentos");
            }
            else
            {
                await this.service.AceptPeticionMedicamentoNuevo(idpeticion, nombre, descripcion, estado);
                return RedirectToAction("AdministradorListaPeticionesMedicamentos");
            }
        }

        [AuthorizeUsers(Policy = "SOLOADMINISTRADOR")]
        public async Task<IActionResult> AdministradorEliminarPeticionMedicamento(int idpeticion)
        {
            await this.service.DeletePeticionMedicamentoAsync(idpeticion);
            return RedirectToAction("AdministradorListaPeticionesMedicamentos");
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
        public async Task<IActionResult> RecepcionistaFindPacienteCita(string nombre, string apellido , string correo , DateTime? fecha , TimeSpan? hora)
        {
            Paciente paciente = await this.service.FindPacienteCitaRecepcionista(nombre,apellido,correo);
            //fecha, hora, idmedico, idpaciente
            if (paciente != null)
            {
                if (fecha != null && hora != null)
                {
                    MedicoDetallado medico = await this.service.GetMedicoPacienteAsync(paciente.Id);
                    await this.service.CreateCitaPacienteAsync(fecha.Value, hora.Value, medico.Id, paciente.Id);
                    return RedirectToAction("RecepcionistaPrincipal");
                }
                else
                {
                    return View(paciente);
                }
            }
            else
            {
                return View(paciente);
            }
        }

        [AuthorizeUsers(Policy = "SOLORECEPCIONISTA")]
        public async Task<IActionResult> RecepcionistaPeticionUsuario()
        {
            ViewData["ESTADOS"] = await this.service.GetEstadosAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RecepcionistaPeticionUsuario(string nombre, string apellido , string correo , int? idpaciente , int? estadonuevo)
        {
            int idrecep = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Paciente paciente = await this.service.FindPacienteCitaRecepcionista(nombre, apellido, correo);
            ViewData["ESTADOS"] = await this.service.GetEstadosAsync();
            if (paciente != null)
            {
                if (idrecep != null && idpaciente != null && estadonuevo != null)
                {
                    await this.service.PeticionUsuarioAsync(idrecep,idpaciente.Value,estadonuevo.Value);
                    return RedirectToAction("RecepcionistaPrincipal");
                }
                else
                {
                    return View(paciente);
                }
                
            }
            else
            {
                ViewData["MENSAJE"] = "Datos erroneos, vuelva a pedir los datos";
                return View();
            }
            
        }

        // ============ Controller Paciente ============ //
        [AuthorizeUsers(Policy = "SOLOPACIENTE")]
        public async Task<IActionResult> PacientePerfil()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Paciente paciente = await this.service.FindPacienteAsync(id);
            return View(paciente);
        }
        [HttpPost]
        public async Task<IActionResult> PacientePerfil(Paciente paciente)
        {
            await this.service.UpdatePacienteAsync(paciente.Id,paciente.Nombre,paciente.Apellido,paciente.Correo,paciente.Contra,paciente.Telefono,paciente.Direccion,paciente.Edad,paciente.Genero,paciente.EstadoUsuario,paciente.TipoUsuario);
            return RedirectToAction("PacientePrincipal");
        }

        [AuthorizeUsers(Policy = "SOLOPACIENTE")]
        public async Task<IActionResult> PacienteMedicoInformacion()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            MedicoDetallado medico = await this.service.GetMedicoPacienteAsync(id);
            return View(medico);
        }

        [AuthorizeUsers(Policy = "SOLOPACIENTE")]
        public async Task<IActionResult> PacienteCitasPaciente()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            MedicoDetallado medico = await this.service.GetMedicoPacienteAsync(id);
            ViewData["NOMBREMEDICO"] = medico.Nombre + " " + medico.Apellido;
            List<CitaDetalladaMedicos> citas = await this.service.GetCitasPacienteAsync(id);
            return View(citas);
        }

        [AuthorizeUsers(Policy = "SOLOPACIENTE")]
        public async Task<IActionResult> PacienteCreateCita()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Usuario paciente = await this.service.FindUsuarioAsync(id);
            MedicoDetallado medico = await this.service.GetMedicoPacienteAsync(id);
            ViewData["NOMBREMEDICO"] = medico.Nombre + " " + medico.Apellido;
            TempData["IDMEDICO"] = medico.Id;
            return View(paciente);
        }
        [HttpPost]
        public async Task<IActionResult> PacienteCreateCita(int idpaciente, int idmedico , DateTime fecha , TimeSpan hora)
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Usuario paciente = await this.service.FindUsuarioAsync(id);
            MedicoDetallado medico = await this.service.GetMedicoPacienteAsync(id);
            ViewData["NOMBREMEDICO"] = medico.Nombre + " " + medico.Apellido;
            TempData["IDMEDICO"] = medico.Id;
            DateTime hoy = DateTime.Now;
            if (fecha < hoy)
            {
                ViewData["MENSAJE"] = "Has introducido una fecha del pasado";
                return View(paciente);
            }
            else
            {
                int dispo = await this.service.FindCitaDisponible(fecha, hora, idmedico);
                if (dispo == 1)
                {
                    ViewData["OTROMENSAJE"] = "Esta Fecha y Hora no esta disponible";
                    return View(paciente);
                }
                else
                {
                    await this.service.CreateCitaPacienteAsync(fecha, hora, idmedico, idpaciente);
                    return RedirectToAction("PacientePrincipal");
                }
            }
        }

        [AuthorizeUsers(Policy = "SOLOPACIENTE")]
        public async Task<IActionResult> PacienteDetallesCita(int idcita)
        {
            CitaDetalladaMedicos cita = await this.service.FindCitaMedicaPacienteAsync(idcita);
            Usuario user = await this.service.FindUsuarioAsync(cita.IdMedico);
            ViewData["NOMBREMEDICO"] = user.Nombre + " " + user.Apellido;
            return View(cita);
        }

        [AuthorizeUsers(Policy = "SOLOPACIENTE")]
        public async Task<IActionResult> PacienteEditarCita(int idcita)
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            CitaDetalladaMedicos cita = await this.service.FindCitaMedicaPacienteAsync(idcita);
            MedicoDetallado medico = await this.service.GetMedicoPacienteAsync(id);
            ViewData["NOMBREMEDICO"] = medico.Nombre + " " + medico.Apellido;
            return View(cita);
        }
        [HttpPost]
        public async Task<IActionResult> PacienteEditarCita(Cita citamodel)
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            CitaDetalladaMedicos cita = await this.service.FindCitaMedicaPacienteAsync(citamodel.Id);
            MedicoDetallado medico = await this.service.GetMedicoPacienteAsync(id);
            ViewData["NOMBREMEDICO"] = medico.Nombre + " " + medico.Apellido;
            DateTime hoy = DateTime.Now;
            if (citamodel.Fecha < hoy)
            {
                ViewData["MENSAJE"] = "Has introducido una fecha del pasado";
                return View(cita);
            }
            else
            {
                int dispo = await this.service.FindCitaDisponible(citamodel.Fecha, citamodel.Hora, citamodel.Medico);
                if (dispo == 1)
                {
                    ViewData["MENSAJE"] = "Esta Fecha y Hora no esta disponible";
                    return View(cita);
                }
                else
                {
                    await this.service.UpdateCitaPacienteAsync(citamodel.Id, citamodel.Fecha, citamodel.Hora);
                    return RedirectToAction("PacienteCitasPaciente");
                }
            }

        }

        [AuthorizeUsers(Policy = "SOLOPACIENTE")]
        public async Task<IActionResult> PacienteDeleteCita(int idcita)
        {
            await this.service.DeleteCitaAsync(idcita);
            return RedirectToAction("PacienteCitasPaciente");
        }

        [AuthorizeUsers(Policy = "SOLOPACIENTE")]
        public async Task<IActionResult> PacienteMedicamentos()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            List<MedicamentoYPaciente> medicamentos = await this.service.GetMedicamentosPacienteAsync(id);
            return View(medicamentos);
        }

        [AuthorizeUsers(Policy = "SOLOPACIENTE")]
        public async Task<IActionResult> PacienteRetirarMedicamento(int id)
        {
            await this.service.AceptMedicamentoPacienteAsync(id);
            return RedirectToAction("PacienteMedicamentos");
        }

        // ============ Controller Medico ============ //
        [AuthorizeUsers(Policy = "SOLOMEDICO")]
        public async Task<IActionResult> MedicoPerfil()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            Medico medico = await this.service.FindMedicoAsync(id);
            ViewData["ESPECIALIDADES"] = await this.service.GetEspecialidadesAsync();
            return View(medico);
        }
        [HttpPost]
        public async Task<IActionResult> MedicoPerfil(Medico medico)
        {
            await this.service.UpdateMedicoAsync(medico.Id,medico.Nombre,medico.Apellido,medico.Correo,medico.Contra,medico.EstadoUsuario,medico.TipoUsuario,medico.Especialidad);
            return RedirectToAction("MedicoPrincipal");
        }

        [AuthorizeUsers(Policy = "SOLOMEDICO")]
        public async Task<IActionResult> MedicoMisPacientes()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            List<MedicosPacientes> mispaciente = await this.service.GetPacientesMedicoAsync(id);
            return View(mispaciente);
        }

        [AuthorizeUsers(Policy = "SOLOMEDICO")]
        public async Task<IActionResult> MedicoInfoPaciente(int idpaciente)
        {
            PacienteDetallado paciente = await this.service.GetPacienteDetallado(idpaciente);
            return View(paciente);
        }

        [AuthorizeUsers(Policy = "SOLOMEDICO")]
        public async Task<IActionResult> MedicoCitasMedicoPaciente()
        {
            int id = int.Parse(this.HttpContext.User.FindFirst("ID").Value);
            List<CitaDetalladaMedicos> citas = await this.service.GetCitasMedicoAsync(id);
            return View(citas);
        }

        [AuthorizeUsers(Policy = "SOLOMEDICO")]
        public async Task<IActionResult> MedicoCitaFinal(int idcita)
        {
            ViewData["MEDICAMENTOS"] = await this.service.GetMedicamentosAsync();
            ViewData["ESTADOSEGUIMIENTO"] = await this.service.GetEstadosSeguimientoAsync();
            CitaDetalladaMedicos cita = await this.service.FindCitaMedicaPacienteAsync(idcita);
            return View(cita);
        }
        [HttpPost]
        public async Task<IActionResult> MedicoCitaFinal(int idmedico , int idpaciente, int idcita , string comentario , int seguimiento , List<int> medicamentos)
        {
            await this.service.FinishCitaMedicaPacienteAsync(idcita,idmedico,idpaciente,comentario,seguimiento,medicamentos);
            ViewData["MEDICAMENTOS"] = await this.service.GetMedicamentosAsync();
            ViewData["ESTADOSEGUIMIENTO"] = await this.service.GetEstadosSeguimientoAsync();
            CitaDetalladaMedicos cita = await this.service.FindCitaMedicaPacienteAsync(idcita);
            if (medicamentos.Count() != 0)
            {
                Usuario paciente = await this.service.FindUsuarioAsync(idpaciente);
                string mensaje = await this.service.ConvertToStringIntAsync(medicamentos);
                await this.service.SendEmailAsync(paciente.Correo, mensaje);
                return RedirectToAction("MedicoPrincipal");
            }
            else
            {
                return RedirectToAction("MedicoPrincipal");
            }
        }

    }
}
