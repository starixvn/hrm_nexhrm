namespace Fanex.NexHR.WebSite
{
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.NexHR.Core;
    using Microsoft.AspNetCore.DataProtection;
    using StackExchange.Redis;

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

            services.RegisterServices();

            var appSettings = Configuration.GetSection("AppSettings").Get<AppSetting>();
            services.AddSingleton(appSettings);

            var redisConnectionString = Configuration.GetSection("Redis:ConnectionString").Value;
            var appName = Configuration.GetSection("Redis:ApplicationName").Value;
            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddDataProtection()
                .SetApplicationName(appName)
                .PersistKeysToStackExchangeRedis(redis, $"{appName}:DataProtection:Keys");
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseCookiePolicy();
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