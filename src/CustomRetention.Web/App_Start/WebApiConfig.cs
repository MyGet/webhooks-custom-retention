using System.Net.Http.Headers;
using System.Web.Http;
using CustomRetention.Web.Models;

namespace CustomRetention.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.Add(
                new TypedJsonMediaTypeFormatter(
                     typeof(WebHookEvent),
                     new MediaTypeHeaderValue("application/vnd.myget.webhooks.v1.preview+json")));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
