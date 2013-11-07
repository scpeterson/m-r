using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SimpleCQRS.Api.Concurrency
{
    public class ConcurrencyAwareFilterAttribute : ActionFilterAttribute
    {

        public ConcurrencyAwareFilterAttribute()
            : this(0, false)
        {

        }

        public ConcurrencyAwareFilterAttribute(int maxAgeInSeconds, bool isPublic)
        {
            _isPublic = isPublic;
            _maxAgeInSeconds = maxAgeInSeconds;
        }

        // Disclaimer: this is not recommended secure encryption. Just for demo purposes
        private static byte[] IV = new byte[] { 
            134, 209, 1, 34, 108, 89, 23, 42 ,
            134, 209, 1, 34, 118, 89, 23, 42
        };

        // Disclaimer: this is not recommended secure encryption. Just for demo purposes
        private static byte[] RGB = new byte[] { 
            134, 209, 1, 34, 108, 89, 23, 42 ,            
            134, 209, 1, 34, 108, 19, 23, 42
        };

        private int _maxAgeInSeconds;
        private bool _isPublic;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // only for PUT where if-match header contains a value
            if (actionContext.Request.Method.Method != HttpMethod.Put.Method ||
                actionContext.Request.Headers.IfMatch == null ||
                actionContext.Request.Headers.IfMatch.Count == 0)

                return;


            foreach (var aa in actionContext.ActionArguments.Values)
            {
                var concurrencyAware = aa as IConcurrencyAware;
                if (concurrencyAware != null)
                {
                    // we take only the first ONLY!
                    try
                    {
                        var decrypted = Decrypt(actionContext.Request.Headers.IfMatch.First().Tag.Trim('"'));
                        concurrencyAware.ConcurrencyVersion = decrypted.Trim();

                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.ToString());
                    }
                }
            }


            base.OnActionExecuting(actionContext);

        }



        // Adding ETag
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

            base.OnActionExecuted(actionExecutedContext);

            // ONLY GET
            if (actionExecutedContext.Request.Method.Method.Equals(HttpMethod.Get.Method) &&
                actionExecutedContext.Response.Content != null &&
                actionExecutedContext.Response.Headers.ETag == null)
            {
                string eTag = null;
                var objectContent = actionExecutedContext.Response.Content as ObjectContent;
                if (objectContent != null)
                {
                    var concurrencyAware = objectContent.Value as IConcurrencyAware;

                    if (concurrencyAware != null)
                    {
                        if (concurrencyAware.ConcurrencyVersion != null)
                        {
                            var eTagString = Encrypt(concurrencyAware.ConcurrencyVersion);
                            eTag = "\"" + eTagString + "\"";
                        }
                    }
                }

                if (eTag != null)
                {
                    SetEtagAndCacheControlHeader(actionExecutedContext, eTag, _maxAgeInSeconds, _isPublic);
                }
            }

        }

        private void SetEtagAndCacheControlHeader(HttpActionExecutedContext context,
                                                  string eTag, int maxAge, bool isPublic)
        {
            var url = context.Request.RequestUri.PathAndQuery;


            // return not modified for conditional GET
            if (context.Request.Headers.IfNoneMatch != null &&
                context.Request.Headers.IfNoneMatch.Any(etgh =>
                etgh.Tag == eTag))
            {
                context.Response = context.Request.CreateResponse(HttpStatusCode.NotModified);
                context.Response.Headers.ETag = new EntityTagHeaderValue(eTag);
                return; // EXIT !!
            }


            context.Response.Headers.ETag = new EntityTagHeaderValue(eTag);
            context.Response.Headers.CacheControl =
                new CacheControlHeaderValue()
                {
                    MaxAge = TimeSpan.FromSeconds(maxAge),
                    Private = !isPublic
                };

        }

        private static string Decrypt(string base64)
        {

            using (var provider = new AesCryptoServiceProvider())
            {
                using (var transform = provider.CreateDecryptor(RGB, IV))
                {
                    var buffer = Convert.FromBase64String(base64);
                    var finalBlock = transform.TransformFinalBlock(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(finalBlock).Trim();
                }
            }
        }

        private static string Encrypt(string data)
        {
            using (var provider = new AesCryptoServiceProvider())
            {
                using (var transform = provider.CreateEncryptor(RGB, IV))
                {
                    var buffer = Encoding.UTF8.GetBytes(data.PadRight(16));
                    var finalBlock = transform.TransformFinalBlock(buffer, 0, buffer.Length);
                    return Convert.ToBase64String(finalBlock);
                }
            }

        }

    }
}