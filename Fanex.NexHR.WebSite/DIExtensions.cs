namespace Fanex.NexHR.WebSite
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Email;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RestSharp;

    public static class DIExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            RegisterAuthentication(services);

            services.AddHttpClient();

            InjectRepository(services);

            InjectServices(services);
        }

        private static void RegisterAuthentication(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(10);
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(cookieOptions =>
            {
                cookieOptions.LoginPath = "/Auth/Login";
                cookieOptions.Cookie.Name = ".HRMSystemNameCookie";
            });
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
            services.AddSingleton<IRestClient, RestClient>();
            services.AddSingleton<IJobTitleService, JobTitleService>();
            services.AddSingleton<IShiftService, ShiftService>();
            services.AddSingleton<IHistoryWorkService, HistoryWorkService>();
            services.AddSingleton<IMailEmployeeService, MailEmployeeService>();

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