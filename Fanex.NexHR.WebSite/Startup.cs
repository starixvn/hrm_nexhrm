namespace Fanex.NexHR.WebSite
{
    using AspNetCore.Unobtrusive.Ajax;
    using Fanex.Data.SqlServer;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using GO.UserControl.Client.Core;
    using GO.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Middlewares;

    public class Startup(IConfiguration configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            services.AddControllersWithViews();

            services.AddFanexLogSentry(configuration);
            services.AddFanexSql(configuration);

            services.Configure<UserControlClientOptions>(configuration.GetSection("GO.UserControl.Client"));

            services.AddHttpClient();
            services.AddHttpClient(Constants.Fnb_Client);
            services.RegisterAuthentication(configuration);
            services.AddHttpContextAccessor();
            services.RegisterServices(configuration);
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

            UserControlClient
                .UseSession(app.ApplicationServices.GetRequiredService<IUserControlSession>(), configuration);

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