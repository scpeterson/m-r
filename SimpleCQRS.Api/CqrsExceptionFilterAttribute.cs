using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;

namespace SimpleCQRS.Api
{
    public class CqrsExceptionFilterAttribute : ExceptionFilterAttribute 
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);
            if (actionExecutedContext.Exception is ConcurrencyException)
            {
                actionExecutedContext.Response =
                    actionExecutedContext.Request.CreateResponse(HttpStatusCode.PreconditionFailed);
                return;
            } 
        }

    }
}