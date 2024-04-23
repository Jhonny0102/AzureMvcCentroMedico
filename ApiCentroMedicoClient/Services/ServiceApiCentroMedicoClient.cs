using ApiCentroMedicoClient.Models;
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

        //Sobre carga de Metodo.
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

        //Sobrecarga de otro metodo.
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

        // ==================== Metodos Login ==================== //

        //Metodo para obtener informacion basica de USUARIO mediante el correo y la contra(NO TOKEN).
        public async Task<Usuario> FindUsuario(string correo , string contra)
        {
            string request = "api/usuarios/findusuariolowdatos/"+correo+"/"+contra;
            Usuario usuario = await this.CallApiAsync<Usuario>(request);
            return usuario;
        }

        // ==================== Metodos Recepcionistas ==================== //

        //Metodo para encontrar el/la recepcionista.
        public async Task<Usuario> FindRecepcionista(int id)
        {
            string request = "api/usuarios/findusuario/"+id;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Usuario recepcionista = await this.CallApiAsync<Usuario>(request,token);
            return recepcionista;
        }

        //Metodo para editar el/la recepcionista.
        public async Task PutRecepcionista(int id, string nombre , string apellido , string correo , string contra , int estado , int tipo)
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

        //Metodo para encontrar un paciente por su nombre , apellido y correo.
        public async Task<Paciente> FindPacienteCitaRecepcionista(string nombre , string apellido , string correo)
        {
            string request = "api/recepcionistas/getpacienterecepcion/"+ nombre +"/"+ apellido +"/" +correo;
            string token = this.httpContextAccessor.HttpContext.User.FindFirst(z => z.Type == "TOKEN").Value;
            Paciente paciente = await this.CallApiAsync<Paciente>(request,token);
            return paciente;
        }

        // ==================== Metodos Administradores ==================== //
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            string request = "api/usuarios/getusuarios";
            List<Usuario> usuarios = await this.CallApiAsync<List<Usuario>>(request);
            return usuarios;
        }

        // ==================== Metodos Pacientes ==================== //


        // ==================== Metodos Medicos ==================== //

    }
}
