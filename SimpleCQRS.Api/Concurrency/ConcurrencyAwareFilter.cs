using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SimpleCQRS.Api.Concurrency
{
    public class ConcurrencyAwareFilter : ActionFilterAttribute
    {
       
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
             // only for PUT where if-match header contains a value
            if(actionContext.Request.Method.Method != HttpMethod.Put.Method ||
                actionContext.Request.Headers.IfMatch == null ||
                actionContext.Request.Headers.IfMatch.Count == 0)

                return;
            

            foreach (var aa in actionContext.ActionArguments.Values)
            {
                var concurrencyAware = aa as IConcurrencyAware;
                if (concurrencyAware != null)
                    concurrencyAware.ConcurrencyVersion = actionContext.Request.Headers.IfMatch.First().Tag; // we take only the first ONLY!
            }


            base.OnActionExecuting(actionContext);

        }
    }
}