namespace NexHRM.AutoJobs
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NexHRM.AutoJobs.Middlewares;
    using NexHRM.AutoJobs.Models;
    using NexHRM.AutoJobs.Services;
    using NexHRM.AutoJobs.Workers;

    public class Startup(IConfiguration configuration)
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFanexSql(configuration);

            services.AddFanexLogSentry(configuration);

            services.AddSingleton(configuration);

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
            var jobSettings = configuration.GetSection("JobSettings").Get<List<JobSetting>>();
            services.AddSingleton(jobSettings);

            var userControlSettings = configuration.GetSection("UserControlSettings").Get<UserControlSettings>();
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