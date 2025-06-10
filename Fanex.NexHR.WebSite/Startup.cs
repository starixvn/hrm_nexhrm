namespace Fanex.NexHR.WebSite
{
    using AspNetCore.Unobtrusive.Ajax;
    using Data.SqlServer;
    using Middlewares;
    using Logging;
    using Fanex.Logging.Sentry;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Sentry;

    public class Startup(IConfiguration configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            services.AddControllersWithViews();

            LogManager
                .SetDefaultLogCategory(configuration.GetSection("LogCategory").Value)
                .Use(new List<ILogEngine>
                {
                    new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(configuration.GetSection("SentryEngineOptions:Dsn").Value),
                        FrontEndDsn = new Dsn(configuration.GetSection("SentryEngineOptions:FrontEndDsn").Value)
                    })
                });

            services.AddFanexSql(configuration);

            services.AddHttpClient();
            services.RegisterAuthentication(configuration);
            services.RegisterServices(Configuration);
            services.InjectConfiguration(configuration);
            services.AddUnobtrusiveAjax();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandler>();
                app.UseHsts();
            }

            app.UseCookiePolicy();
            app.UseStaticFiles();

            if (configuration.GetValue<bool>("MAINTENANCE_MODE"))
            {
                app.Run(async (context) =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.SendFileAsync("wwwroot/maintenance.html");
                });
            }

            app.UseAuthentication();
            app.UseSession();

            app.UseRouting();
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/sync"), branch =>
            {
                branch.UseMiddleware<ApiKeyMiddleware>();
            });

            app.UseAuthorization();

            app.UseUnobtrusiveAjax();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "api",
                    pattern: "api/{controller=Token}/{action=GetToken}/{id?}");
            });
        }
    }
}