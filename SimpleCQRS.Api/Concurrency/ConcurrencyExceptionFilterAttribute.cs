﻿using System.Net;
using System.Web.Http.Filters;
using System.Net.Http;

namespace SimpleCQRS.Api.Concurrency
{
    public class ConcurrencyExceptionFilterAttribute : ExceptionFilterAttribute 
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);
            if (actionExecutedContext.Exception is ConcurrencyException)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.PreconditionFailed);
                return;
            } 
        }

    }
}