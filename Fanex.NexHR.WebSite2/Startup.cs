namespace Fanex.NexHR.WebSite2
{
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.NexHR.Core;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var connections = Configuration.GetSection("ConnectionStrings")
                                                .GetChildren()
                                                .AsEnumerable()
                                                .Where(x => !string.IsNullOrWhiteSpace(x.GetValue<string>("Name")))
                                                .ToDictionary(x => x.GetValue<string>("Name"),
                                                y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName")));

            DbSettingProviderManager
               .StartNewSession()
               .Use(connections)
               .WithSqlServer()
               .UseDefaultDbSettingProvider(resourcePath: $"~/App_Data/SpNames.xml")
               .Run();

            DIExtensions.RegisterServices(services);

            var appSettings = Configuration.GetSection("AppSettings").Get<AppSetting>();
            services.AddSingleton(appSettings);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}