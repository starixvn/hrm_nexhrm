namespace Fanex.NexHR.WebSite
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Web.Http;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config?.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });

            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            ((DefaultContractResolver)jsonFormatter.SerializerSettings.ContractResolver)
                .IgnoreSerializableAttribute = true;
        }
    }
}