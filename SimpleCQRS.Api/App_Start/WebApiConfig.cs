﻿using System.Web.Http;
using FiveLevelsOfMediaType;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleCQRS.Api.Concurrency;

namespace SimpleCQRS.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            config.AddFiveLevelsOfMediaType();

            config.Filters.Add(new ConcurrencyAwareFilterAttribute());
            config.Filters.Add(new ConcurrencyExceptionFilterAttribute());

        }
    }
}