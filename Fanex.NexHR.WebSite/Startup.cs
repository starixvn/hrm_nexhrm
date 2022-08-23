namespace Fanex.NexHR.WebSite
{
    using AspNetCore.Unobtrusive.Ajax;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.NexHR.Services.Interfaces;
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
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
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

            services.AddHttpClient();
            services.RegisterAuthentication(Configuration);
            services.RegisterServices();
            services.InjectConfiguration(Configuration);
            services.AddUnobtrusiveAjax();

            TestSendEmail(services);
        }

        private void TestSendEmail(IServiceCollection services)
        {
            var emailService = services.BuildServiceProvider().GetService<IEmailService>();
            emailService.SendEmail("nexhr@nexcel.com", "oliver.dang@nexcel.com", "", "Test", "Oliver test");
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

            app.UseUnobtrusiveAjax();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}