﻿using Autofac;
using Autofac.Integration.Owin;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Voyage.Services.Client;

namespace Voyage.Security.Oauth2.Controllers
{
    public class OAuthController : Controller
    {

        public async Task<ActionResult> Authorize()
        {
            var context = HttpContext.GetOwinContext().GetAutofacLifetimeScope();
            var clientService = context.Resolve<IClientService>();

            if (Response.StatusCode != 200)
            {
                return View("AuthorizeError");
            }

            var authentication = HttpContext.GetOwinContext().Authentication;
            var ticket = authentication.AuthenticateAsync("Application").Result;
            var identity = ticket?.Identity;
            if (identity == null)
            {
                authentication.Challenge("Application");
                return new HttpUnauthorizedResult();
            }

            // TODO: Get specific client's scopes.
            var scopes = await clientService.GetScopeListByClientId(Request.Params["client_id"]);
            ViewBag.Scopes = scopes;

            if (Request.HttpMethod != "POST")
                return View();

            if (!string.IsNullOrEmpty(Request.Form.Get("submit.Grant")))
            {
                identity = new ClaimsIdentity(identity.Claims, "Bearer", identity.NameClaimType, identity.RoleClaimType);

                foreach (var scope in scopes)
                {
                    identity.AddClaim(new Claim("urn:oauth:scope", scope));
                }

                authentication.SignIn(identity);
            }

            if (!string.IsNullOrEmpty(Request.Form.Get("submit.Login")))
            {
                authentication.SignOut("Application");
                authentication.Challenge("Application");
                return new HttpUnauthorizedResult();
            }

            return View();
        }
    }
}