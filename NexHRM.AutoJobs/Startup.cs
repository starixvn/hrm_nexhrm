namespace NexHRM.AutoJobs
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NexHRM.AutoJobs.Middlewares;
    using NexHRM.AutoJobs.Models;
    using NexHRM.AutoJobs.Services;
    using NexHRM.AutoJobs.Workers;
    using Sentry;

    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFanexSql(Configuration);

            LogManager
                .SetDefaultLogCategory(Configuration.GetSection("SentrySettings:LogCategory").Value)
                .Use(new List<ILogEngine>
                {
                    new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(Configuration.GetSection("SentrySettings:Dsn").Value)
                    })
                });

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .GetHttpHeaderInfo()
                .GetHttpSessionInfo()
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            services.AddSingleton(Configuration);

            services.AddHttpClient();
            AddDI(services);
            AddWorker(services);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandlerMiddleware>();
                app.UseHsts();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void AddDI(IServiceCollection services)
        {
            var jobSettings = Configuration.GetSection("JobSettings").Get<List<JobSetting>>();
            services.AddSingleton(jobSettings);

            var userControlSettings = Configuration.GetSection("UserControlSettings").Get<UserControlSettings>();
            services.AddSingleton(userControlSettings);

            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddSingleton<IBaseService, BaseService>();

            services.AddSingleton<IGroupReportService, GroupReportService>();
        }

        private static void AddWorker(IServiceCollection services)
        {
            services.AddHostedService<SyncEmployeeInfoToGroupReportWorker>();
        }
    }
}