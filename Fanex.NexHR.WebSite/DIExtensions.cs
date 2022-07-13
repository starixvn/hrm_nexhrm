namespace Fanex.NexHR.WebSite
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models.NexBot;
    using Fanex.NexHR.Models.SyncData;
    using Fanex.NexHR.Models.Token;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Email;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using StackExchange.Redis;

    public static class DIExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            InjectRepository(services);

            InjectServices(services);
        }

        public static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetSection("Redis:ConnectionString").Value;
            var appName = configuration.GetSection("Redis:ApplicationName").Value;

            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddDataProtection()
                .SetApplicationName(appName)
                .PersistKeysToStackExchangeRedis(redis, $"{appName}:DataProtection:Keys");

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(10);
                options.Cookie.IsEssential = true;
                options.Cookie.Name = $"{appName}.Session";
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = $"{appName}:Session:";
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(cookieOptions =>
            {
                cookieOptions.LoginPath = "/Auth/Login";
                cookieOptions.ExpireTimeSpan = TimeSpan.FromDays(10);
                cookieOptions.Cookie = new CookieBuilder
                {
                    Name = $"{appName}.Cookie",
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
            });
        }

        public static void InjectConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection("AppSettings").Get<AppSetting>();
            services.AddSingleton(appSettings);

            var nexBotSettings = configuration.GetSection("NexBotSettings").Get<NexBotSettings>();
            services.AddSingleton(nexBotSettings);

            var syncDataSettings = configuration.GetSection("SyncDataSettings").Get<SyncDataSettings>();
            services.AddSingleton(syncDataSettings);

            var authSettings = configuration.GetSection("AuthorizedSettings").Get<AuthorizedSettings>();
            services.AddSingleton(authSettings);
        }

        private static void InjectServices(IServiceCollection services)
        {
            services.AddScoped<ISessionService, SessionService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<BaseService>();

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ILogServiceProvider, LogServiceProvider>();
            services.AddSingleton<IEmployeeService, EmployeeService>();
            services.AddSingleton<ITeamService, TeamService>();
            services.AddSingleton<IRequestService, RequestService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<ILateInEarlyOutService, LateInEarlyOutService>();
            services.AddSingleton<ICommandService, CommandService>();
            services.AddSingleton<IRedmineService, RedmineService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<INexbotService, NexbotService>();
            services.AddSingleton<IJobTitleService, JobTitleService>();
            services.AddSingleton<IShiftService, ShiftService>();
            services.AddSingleton<IHistoryWorkService, HistoryWorkService>();
            services.AddSingleton<IMailEmployeeService, MailEmployeeService>();
            services.AddSingleton<ILeaveTypeService, LeaveTypeService>();

            // services.AddSingleton<ILateInEarlyOutRepository, LateInEarlyOutRepository>();
        }

        private static void InjectRepository(IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<ITeamRepository, TeamRepository>();
            services.AddSingleton<IRequestRepository, RequestRepository>();
            services.AddSingleton<INotificationRepository, NotificationRepository>();
            services.AddSingleton<ILateInEarlyOutRepository, LateInEarlyOutRepository>();
            services.AddSingleton<IReportRepository, ReportRepository>();
            services.AddSingleton<INexbotRepository, NexbotRepository>();
        }
    }
}