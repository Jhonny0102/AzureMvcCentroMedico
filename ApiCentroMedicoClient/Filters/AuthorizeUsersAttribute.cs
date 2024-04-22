using ApiCentroMedicoClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiCentroMedicoClient.Filters
{
    public class AuthorizeUsersAttribute : AuthorizeAttribute , IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity.IsAuthenticated == false)
            {
                context.Result = this.GetRoute("Managed","Login");
            }
            else /*Este codigo esta en dudas, mirar si salta error por si es esta zona*/
            {
                if(user.IsInRole("1") == false && user.IsInRole("2") == false && user.IsInRole("3") == false && user.IsInRole("4") == false)
                {
                    context.Result = this.GetRoute("Managed","ErrorAcceso");
                }
            }
        }

        private RedirectToRouteResult GetRoute(string controller, string action)
        {
            RouteValueDictionary ruta = new RouteValueDictionary(new
            {
                controller = controller,
                action = action
            });
            RedirectToRouteResult result = new RedirectToRouteResult(ruta);
            return result;
        }
    }
}

//Esta clase nos permite redirigir automaticamente a los usuarios si este esta logeado o no
