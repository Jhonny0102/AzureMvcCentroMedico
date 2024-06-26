﻿using ApiCentroMedicoClient.Models;
using Azure;
using Azure.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using System.Numerics;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiCentroMedicoClient.Services
{
    public class ServiceApiCentroMedicoClient
    {
        private string UrlApiCentro;
        private MediaTypeWithQualityHeaderValue Header;
        private IHttpContextAccessor httpContextAccessor;
        public ServiceApiCentroMedicoClient(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.UrlApiCentro = configuration.GetValue<string>("ApiUrls:ApiCentroMedico");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.httpContextAccessor = httpContextAccessor;
        }

        //Metodo para enviar Email al paciente.
        public async Task SendEmailAsync(string email, string mensaje)
        {
            string urlLogicApp = "https://prod-205.westeurope.logic.azure.com:443/workflows/76b8f2f46f144844adf0136c01e543ad/triggers/When_a_HTTP_request_is_received/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2FWhen_a_HTTP_request_is_received%2Frun&sv=1.0&sig=7lIQOb0cfeMe-W6YUz21_WIXrWBc9PHpEoQe0unHt6A";
            var model = new
            {
                email = email,
                asunto = "Cita finalizada Medico",
                mensaje = mensaje
            };
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(urlLogicApp, content);
            }
        }
        //Metodo para convertir los ids de los medicamentos en un string
        public async Task<string> ConvertToStringIntAsync(List<int> idsmedicamentos)
        {
            string data = this.TransformCollectionToQuery(idsmedicamentos);
            string request = "api/otros/getmensaje";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            string mensaje = await this.CallApiAsync<string>(request + "?" + data, token);
            return mensaje;
        }


        //Metodo que nos recupera el token.
        public async Task<string> GetTokenAsync(string correo , string contra)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                Login model = new Login
                {
                    Correo = correo,
                    Contra = contra
                };
                string jsonData = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, this.Header);
                HttpResponseMessage response = await client.PostAsync(request,content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        // *** Metodo Get *** //
        //1.Metodo primero, solo recibe el request.
        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        //2.Metodo segundo. Recibe el request y el token
        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization","bearer "+token);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        // *** Metodo Put *** //
        //1. Metodo para actualizar(PUT) donde se pasa request , generico(T).
        private async Task CallPutApiAsync<T>(string request, T objeto)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                string json = JsonConvert.SerializeObject(objeto);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);
            }
        }
        //2. Metodo para actualizar(PUT) donde se pasa request , generico(T) , token.
        private async Task CallPutApiAsync<T>(string request, T objeto, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                string json = JsonConvert.SerializeObject(objeto);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);
            }
        }

        // *** Metodo Delete *** // 
        //1. Metodo Delete donde pasamos el request(url del API).
        private async Task CallDeleteAsync(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }
        //2. Metodo delete donde pasamos el request y el token.
        private async Task CallDeleteAsync(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        // *** Metodo Post *** //
        private async Task CallPostAsync<T>(string request , T objeto)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                string json = JsonConvert.SerializeObject(objeto);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        private async Task CallPostAsync<T>(string request, T objeto, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                string json = JsonConvert.SerializeObject(objeto);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        private async Task CallPostAsync(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.PostAsync(request,null);
            }
        }



        // ==================== Metodos Login ==================== //

        //Metodo para obtener informacion basica de USUARIO mediante el correo y la contra(NO TOKEN).
        public async Task<Usuario> FindUsuario(string correo , string contra)
        {
            string request = "api/usuarios/findusuariolowdatos/"+correo+"/"+contra;
            Usuario usuario = await this.CallApiAsync<Usuario>(request);
            return usuario;
        }

        //Metodo para crear paciente desde el login.
        public async Task CreatePacienteAsync(string nombre, string apellido, string correo, string contra, int telefono, string direccion, int edad, string genero,int especialidad)
        {
            string request = "api/pacientes/createpaciente/"+especialidad;
            Paciente user = new Paciente();
            user.Nombre = nombre;
            user.Apellido = apellido;
            user.Correo = correo;
            user.Contra = contra;
            user.Telefono = telefono;
            user.Direccion = direccion;
            user.Edad = edad;
            user.Genero = genero;
            this.CallPostAsync(request, user);
        }

        // ==================== Metodos Comun Recepcionistas Y Administradores ==================== //

        //Metodo para encontrar el/la usuario.
        public async Task<Usuario> FindUsuarioAsync(int id)
        {
            string request = "api/usuarios/findusuario/" + id;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Usuario user = await this.CallApiAsync<Usuario>(request, token);
            return user;
        }

        //Metodo para editar el/la usuario.
        public async Task PutUsuarioAsync(int id, string nombre, string apellido, string correo, string contra, int estado, int tipo)
        {
            string request = "api/usuarios";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Usuario user = new Usuario();
            user.Id = id;
            user.Nombre = nombre;
            user.Apellido = apellido;
            user.Correo = correo;
            user.Contra = contra;
            user.Id_EstadoUsuario = estado;
            user.Id_TipoUsuario = tipo;
            this.CallPutApiAsync(request, user, token);
        }

        // ==================== Metodos Recepcionistas ==================== //

        //Metodo para encontrar un paciente por su nombre , apellido y correo.
        public async Task<Paciente> FindPacienteCitaRecepcionista(string nombre , string apellido , string correo)
        {
            string request = "api/recepcionistas/getpacienterecepcion/"+ nombre +"/"+ apellido +"/" +correo;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Paciente paciente = await this.CallApiAsync<Paciente>(request,token);
            return paciente;
        }
        //Metodo para solicitar una peticion de usuaio.
        public async Task PeticionUsuarioAsync(int idrecep , int idpaciente , int estadonuevo)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/recepcionistas/createpeticionusuario/" + idrecep + "/" + idpaciente + "/" + estadonuevo;
                string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.PostAsync(request, null);
            }
        }



        // ==================== Metodos Administradores ==================== //

        //Metodo para recuperar todos los usuarios.
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            string request = "api/usuarios/getusuarios";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<Usuario> usuarios = await this.CallApiAsync<List<Usuario>>(request,token);
            return usuarios;
        }
        //Metodo para recuperar todos los usuarios de un tipo especifico. Por ejemplo todos los usuario que sean paciente,recepcionista,medico o adminisitrador.
        public async Task<List<Usuario>> GetUsuariosTipoAsync(int tipo)
        {
            string request = "api/usuarios/getusuariostipo/"+tipo;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<Usuario> usuarios = await this.CallApiAsync<List<Usuario>>(request,token);
            return usuarios;
        }
        //Metodo para recuperar una lista de los diferentes tipos de usuario(admin,recepcionista,paciente,medico).
        public async Task<List<UsuariosTipo>> GetTipoUsuario()
        {
            string request = "api/usuarios/gettipousuarios";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<UsuariosTipo> tipos = await this.CallApiAsync<List<UsuariosTipo>>(request,token);
            return tipos;
        }
        //Metodo para ver todos los datos de un paciente.
        public async Task<PacienteDetallado> GetPacienteDetallado(int idpaciente)
        {
            string request = "api/pacientes/findpacientedetallado/"+idpaciente;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            PacienteDetallado paciente = await this.CallApiAsync<PacienteDetallado>(request,token);
            return paciente;
        }
        //Metodo para ver todos los datos de un medico.
        public async Task<MedicoDetallado> GetMedicoDetallado(int idmedico)
        {
            string request = "api/medicos/findmedicodetallado/" + idmedico;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            MedicoDetallado medico = await this.CallApiAsync<MedicoDetallado>(request, token);
            return medico;
        }
        //Metodo para ver todos los datos de un usuario(Administrador y Recepcionista).
        public async Task<UsuarioDetallado> GetUsuarioDetallado(int idusuario)
        {
            string request = "api/usuarios/getusuariodetallado/" + idusuario;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            UsuarioDetallado usuario = await this.CallApiAsync<UsuarioDetallado>(request, token);
            return usuario;
        }
        //Metodo para encontrar un medico.
        public async Task<Medico> FindMedicoAsync(int idmedico)
        {
            string request = "api/medicos/findmedico/" + idmedico;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Medico medico = await this.CallApiAsync<Medico>(request, token);
            return medico;
        }
        //Metodo para actualizar los datos de un medico.
        public async Task UpdateMedicoAsync(int idmedico, string nombre, string apellido, string correo, string contra, int estado, int tipo, int especialidad)
        {
            string request = "api/medicos/updatemedico";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Medico user = new Medico();
            user.Id = idmedico;
            user.Nombre = nombre;
            user.Apellido = apellido;
            user.Correo = correo;
            user.Contra = contra;
            user.EstadoUsuario = estado;
            user.TipoUsuario = tipo;
            user.Especialidad = especialidad;
            this.CallPutApiAsync(request, user, token);
        }
        //Metodo para encontrar un paciente.
        public async Task<Paciente> FindPacienteAsync(int idpaciente)
        {
            string request = "api/pacientes/findpaciente/" + idpaciente;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Paciente paciente = await this.CallApiAsync<Paciente>(request, token);
            return paciente;
        }
        //Metodo para actualizar los datos de un paciente.
        public async Task UpdatePacienteAsync(int idpaciente, string nombre, string apellido, string correo, string contra, int telefono, string direccion, int edad, string genero, int estado, int tipo)
        {
            string request = "api/pacientes";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Paciente user = new Paciente();
            user.Id = idpaciente;
            user.Nombre = nombre;
            user.Apellido = apellido;
            user.Correo = correo;
            user.Contra = contra;
            user.EstadoUsuario = estado;
            user.TipoUsuario = tipo;
            user.Telefono = telefono;
            user.Direccion = direccion;
            user.Edad = edad;
            user.Genero = genero;
            this.CallPutApiAsync(request, user, token);
        }
        //Metodo para obtener el conjunto de estado(BAJA/ALTA)
        public async Task<List<Estados>> GetEstadosAsync()
        {
            string request = "api/otros/getestados";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<Estados> estados = await this.CallApiAsync<List<Estados>>(request, token);
            return estados;
        }
        //Metodo para obtener el conjunto de especialidades de un medico.
        public async Task<List<Especialidades>> GetEspecialidadesAsync()
        {
            string request = "api/otros/getespecialidadesmedico";
            List<Especialidades> especialidades = await this.CallApiAsync<List<Especialidades>>(request);
            return especialidades;
        }
        //Metodo para eliminar un usuario.
        public async Task DeleteUsuarioAsync(int idusuario , int idtipo)
        {
            string request = "api/usuarios/deleteusuario/"+idusuario+"/"+idtipo;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            this.CallDeleteAsync(request,token);
        }
        //Metodo para crear un Medico desde Administrador.
        public async Task CreateMedicoAsync(string nombre , string apellido , string correo , string contra , int especialidad)
        {
            string request = "api/medicos/createmedico";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Medico user = new Medico();
            user.Nombre = nombre;
            user.Apellido = apellido;
            user.Correo = correo;
            user.Contra = contra;
            user.Especialidad = especialidad;
            this.CallPostAsync(request, user, token);
        }
        //Metodo para crear un Recepcionista desde Administrador.
        public async Task CreateUsuarioAsync(string nombre, string apellido, string correo, string contra, int tipo)
        {
            string request = "api/usuarios/createusuario";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Usuario user = new Usuario();
            user.Nombre = nombre;
            user.Apellido = apellido;
            user.Correo = correo;
            user.Contra = contra;
            user.Id_TipoUsuario = tipo;
            this.CallPostAsync(request, user, token);
        }
        //Metodo para obtener el conjunto de citas.
        public async Task<List<CitaDetallado>> GetCitasFiltroAsync(DateTime fecha , DateTime? fechahasta)
        {
            string request = "api/administrador/getcitas";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<CitaDetallado> citas = await this.CallApiAsync<List<CitaDetallado>>(request, token);
            //Si ha pasado el parametro fechahasta sera un filtro de rango.
            if (fechahasta != null)
            {
                return citas.Where(z => z.Fecha >= fecha && z.Fecha <= fechahasta).ToList();
            }
            //Si solo ha pasado la fecha sera fechas en adelante.
            else
            {
                return citas.Where(z => z.Fecha >= fecha).ToList();
            }
        }
        //Metodo para obtener todas las citas.
        public async Task<List<CitaDetallado>> GetCitasAsync()
        {
            string request = "api/administrador/getcitas";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<CitaDetallado> citas = await this.CallApiAsync<List<CitaDetallado>>(request, token);
            return citas;
        }
        //Metodo para encontrar una CITA.
        public async Task<Cita> FindCitaAsync(int idcita)
        {
            string request = "api/administrador/findcita/"+idcita;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Cita cita = await this.CallApiAsync<Cita>(request, token);
            return cita;
        }
        //Metodo para encontrar un estado de cita mediante id.
        public async Task<SeguimientoCita> FindSeguimientoCitaAsync(int idseguimientocita)
        {
            string request = "api/otros/getestadosseguimiento";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<SeguimientoCita> seguimientos = await this.CallApiAsync<List<SeguimientoCita>>(request, token);
            return seguimientos.FirstOrDefault(z => z.Id == idseguimientocita);
        }
        //Metodo para eliminar una cita.
        public async Task DeleteCitaAsync(int idcita)
        {
            string request = "api/administrador/deletecita/"+idcita;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            this.CallDeleteAsync(request, token);
        }
        //Metodo para obtener todas las peticiones sobre usuarios.
        public async Task<List<PeticionesDetallado>> GetPeticionesUsuarioAsync()
        {
            string request = "api/administrador/getpeticionesdetalladasusuarios";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<PeticionesDetallado> peticionesUsuarios = await this.CallApiAsync<List<PeticionesDetallado>>(request, token);
            return peticionesUsuarios;
        }
        //Metodo para aceptar una peticion de usuario.
        public async Task AceptPeticionUsuarioAsync(int idpeticion, int idusuario , int estado)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/administrador/aceptarpeticionusuario/"+idpeticion+"/"+idusuario+"/"+estado;
                string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.PutAsync(request, null);
            }
        }
        //Metodo para denegar una peticion de usuario.
        public async Task DeletePeticionUsuarioAsync(int idpeticion)
        {
            string request = "api/administrador/eliminarpeticionusuario/" + idpeticion;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            this.CallDeleteAsync(request, token);
        }


        //Metodo para aceptar una peticion de medicamento nuevo.
        public async Task AceptPeticionMedicamentoNuevo(int idpeticion, string nombre , string descripcion, int estado)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/administrador/aceptarpeticionmedicamentonuevo/"+idpeticion+"/"+nombre+"/"+descripcion+"/"+estado;
                string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.PutAsync(request,null);
            }
        }
        //Metodo para aceptar una peticion de medicamento actualizado.
        public async Task AceptPeticionMedicamentoUpdate(int idpeticion, int idmedicamento , int estado)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/administrador/aceptarpeticionmedicamentoactualizado/"+idpeticion+"/"+idmedicamento+"/"+estado;
                string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.PutAsync(request, null);
            }
        }
        //Metodo para obtener todas las peticiones sobre los medicamentos.
        public async Task<List<PeticionesMedicamentoDetallado>> GetPeticionesMedicamentosAsync()
        {
            string request = "api/administrador/getpeticionesdetalladasmedicamentos";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<PeticionesMedicamentoDetallado> peticionesMedicamentos = await this.CallApiAsync<List<PeticionesMedicamentoDetallado>>(request, token);
            return peticionesMedicamentos;
        }
        //Metodo para eliminar una peticion de medicamentos.
        public async Task DeletePeticionMedicamentoAsync(int idpeticion)
        {
            string request = "api/administrador/eliminarpeticionmedicamento/"+idpeticion;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            this.CallDeleteAsync(request, token);
        }



        // ==================== Metodos Pacientes ==================== //
        //Metodo para encontrar el medico asignado al paciente.
        public async Task<MedicoDetallado> GetMedicoPacienteAsync(int idpaciente)
        {
            string request = "api/pacientes/getmimedico/"+idpaciente;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            MedicoDetallado medico = await this.CallApiAsync<MedicoDetallado>(request, token);
            return medico;
        }
        //Metodo para obtener el conjunto de citas de un paciente.
        public async Task<List<CitaDetalladaMedicos>> GetCitasPacienteAsync(int idpaciente)
        {
            string request = "api/pacientes/findcitaspaciente/" + idpaciente;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<CitaDetalladaMedicos> citas = await this.CallApiAsync<List<CitaDetalladaMedicos>>(request, token);
            return citas.OrderByDescending(z => z.Fecha).ToList();

        }
        //Metodo para crear una cita siendo Paciente.
        public async Task CreateCitaPacienteAsync(DateTime fecha , TimeSpan hora , int idmedico , int idpaciente)
        {
            string request = "api/pacientes/createcitapaciente";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Cita cita = new Cita();
            cita.Fecha = fecha;
            cita.Hora = hora;
            cita.Medico = idmedico;
            cita.Paciente = idpaciente;
            this.CallPostAsync(request, cita, token);
        }
        //Metodo para verificar si una cita esta disponible.
        public async Task<int> FindCitaDisponible(DateTime fecha, TimeSpan hora, int idmedico)
        {
            string request = "api/administrador/getcitasbasica";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<Cita> citas = await this.CallApiAsync<List<Cita>>(request, token);
            Cita cita = citas.FirstOrDefault(z => z.Fecha == fecha && z.Hora == hora && z.Medico == idmedico && z.SeguimientoCita == 3 && z.IdEstado == 1); 
            if (cita != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
            
        }
        //Metodo para buscar una cita medica de un paciente.
        public async Task<CitaDetalladaMedicos> FindCitaMedicaPacienteAsync(int idcita)
        {
            string request = "api/medicos/findcitadetalladamedico/"+idcita;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            CitaDetalladaMedicos cita = await this.CallApiAsync<CitaDetalladaMedicos>(request, token);
            return cita;
        }
        //Metodo para actualizar una cita siendo paciente.
        public async Task UpdateCitaPacienteAsync(int idcita , DateTime fecha , TimeSpan hora)
        {
            string request = "api/pacientes/updatecitapaciente";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Cita cita = new Cita();
            cita.Id = idcita;
            cita.Fecha = fecha;
            cita.Hora = hora;
            this.CallPutApiAsync(request, cita, token);

        }

        

        //Metodo para obtener el conjunto de medicamentos de un paciente.
        public async Task<List<MedicamentoYPaciente>> GetMedicamentosPacienteAsync(int idpaciente)
        {
            string request = "api/pacientes/getmedicamentospaciente/" + idpaciente;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<MedicamentoYPaciente> medicamentos = await this.CallApiAsync<List<MedicamentoYPaciente>>(request, token);
            return medicamentos;
        }
        //Metodo para retirar un medicamento asigando a un paciente.
        public async Task AceptMedicamentoPacienteAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/medicamentos/updatemedicamentoypaciente/"+id;
                string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.PutAsync(request, null);
            }
        }

        // ==================== Metodos Medicos ==================== //
        //Metodo para buscar un medico detallado.
        public async Task<MedicoDetallado> FindMedicoDetalladoAsync(int idmedico)
        {
            string request = "api/medicos/findmedicodetallado/" + idmedico;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            MedicoDetallado medico = await this.CallApiAsync<MedicoDetallado>(request, token);
            return medico;
        }
        //Metodo para obtener todos los pacientes de un medico.
        public async Task<List<MedicosPacientes>> GetPacientesMedicoAsync(int idmedico)
        {
            string request = "api/medicos/getmispaciente/" + idmedico;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<MedicosPacientes> mispacientes = await this.CallApiAsync<List<MedicosPacientes>>(request, token);
            return mispacientes;
        }
        //Metodo para obtener el conjunto de citas de un medico.
        public async Task<List<CitaDetalladaMedicos>> GetCitasMedicoAsync(int idmedico)
        {
            string request = "api/medicos/GetCitasDetalladasMedico/" + idmedico;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<CitaDetalladaMedicos> citas = await this.CallApiAsync<List<CitaDetalladaMedicos>>(request, token);
            return citas;
        }
        //Metodo para transformar una lista de int a una cadena de string.
        private string TransformCollectionToQuery(List<int> collection)
        {
            string result = "";
            foreach (int elem in collection)
            {
                result += "medicamentos=" + elem + "&";
            }
            result = result.TrimEnd('&');
            return result;

        }
        //Metodo para obtener los medicamentos.
        public async Task<List<Medicamentos>> GetMedicamentosAsync()
        {
            string request = "api/medicamentos/getmedicamentos";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<Medicamentos> medicamentos = await this.CallApiAsync<List<Medicamentos>>(request, token);
            return medicamentos.OrderBy(z => z.Nombre).ToList();
        }
        //Metodo para obtener todos los estados.
        public async Task<List<SeguimientoCita>> GetEstadosSeguimientoAsync()
        {
            string request = "api/otros/getestadosseguimiento";
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<SeguimientoCita> estados= await this.CallApiAsync<List<SeguimientoCita>>(request, token);
            return estados;
        }

        //Metodo para finalizar una cita medica paciente.
        public async Task FinishCitaMedicaPacienteAsync(int idcita, int idmedico , int idpaciente , string comentario , int seguimiento , List<int> medicamentos)
        {
            using (HttpClient client = new HttpClient())
            {
                string data = this.TransformCollectionToQuery(medicamentos);
                string request = "api/medicos/updatecitamedica/" + idmedico + "/" + idpaciente + "/" +idcita + "/" + comentario + "/" +seguimiento;
                string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
                client.BaseAddress = new Uri(this.UrlApiCentro);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.PutAsync(request+"?"+data, null);
            }
        }


    }
}
