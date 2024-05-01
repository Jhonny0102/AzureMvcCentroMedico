﻿using ApiCentroMedicoClient.Models;
using Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
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

        // *** Metodo Delete *** // ---->> Poner a prueba
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

        
        
        // ==================== Metodos Login ==================== //

        //Metodo para obtener informacion basica de USUARIO mediante el correo y la contra(NO TOKEN).
        public async Task<Usuario> FindUsuario(string correo , string contra)
        {
            string request = "api/usuarios/findusuariolowdatos/"+correo+"/"+contra;
            Usuario usuario = await this.CallApiAsync<Usuario>(request);
            return usuario;
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
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            List<Especialidades> especialidades = await this.CallApiAsync<List<Especialidades>>(request, token);
            return especialidades;
        }

        //metodo para eliminar un usuario.
        public async Task DeleteUsuarioAsync(int idusuario , int idtipo)
        {
            string request = "api/usuarios/deleteusuario/"+idusuario+"/"+idtipo;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            this.CallDeleteAsync(request,token);
        }



        // ==================== Metodos Pacientes ==================== //


        // ==================== Metodos Medicos ==================== //

    }
}
