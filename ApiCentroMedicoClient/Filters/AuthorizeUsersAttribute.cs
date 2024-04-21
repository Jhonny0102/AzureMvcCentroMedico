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
                RouteValueDictionary routeLogin = new RouteValueDictionary(new
                {
                    controller = "Managed",
                    action = "Login"
                });
                context.Result = new RedirectToRouteResult(routeLogin);
            }
        }
    }
}

//Esta clase nos permite redirigir automaticamente a los usuarios si este esta logeado o no
