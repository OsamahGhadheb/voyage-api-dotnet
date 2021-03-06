﻿using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Voyage.Api.Extensions;

namespace Voyage.Api.Filters
{
    /// <summary>
    /// Returns a BadRequest response if the model is invalid
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext
                    .Request
                    .CreateResponse(
                        HttpStatusCode.BadRequest,
                        actionContext.ModelState.ConvertToResponseModel());
            }
        }
    }
}