using ApiCentroMedicoClient.Models;
using ApiCentroMedicoClient.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiCentroMedicoClient.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceApiCentroMedicoClient service;
        public ManagedController(ServiceApiCentroMedicoClient service)
        {
            this.service = service;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            string token = await this.service.GetTokenAsync(model.Correo,model.Contra);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrecto";
                return View();
            }
            else
            {
                Usuario user = await this.service.FindUsuario(model.Correo, model.Contra);
                HttpContext.Session.SetString("TOKEN",token);
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim("TOKEN", token)); //almacenamos el token dentro del usuario
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Nombre));
                identity.AddClaim(new Claim(ClaimTypes.Role, user.Id_TipoUsuario.ToString()));
                identity.AddClaim(new Claim("ID", user.Id.ToString()));
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                });
                return RedirectToAction("Index","Home");
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }
    }
}
