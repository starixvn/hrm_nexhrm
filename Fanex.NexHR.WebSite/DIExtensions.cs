namespace Fanex.NexHR.WebSite
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models.Core;
    using Fanex.NexHR.Models.NexBot;
    using Fanex.NexHR.Models.SyncData;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.AutoJobService;
    using Fanex.NexHR.Services.Email;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.IdentityModel.Tokens;
    using StackExchange.Redis;
    using System.Text;

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

            string key = configuration.GetSection("AuthorizedSettings:Secret").Value;
            services.AddAuthentication()
                .AddCookie(cookieOptions =>
                {
                    cookieOptions.LoginPath = "/Auth/Login";
                    cookieOptions.ExpireTimeSpan = TimeSpan.FromDays(1);
                    cookieOptions.SlidingExpiration = true;
                    cookieOptions.Cookie = new CookieBuilder
                    {
                        Name = $"{appName}.Cookie",
                        SecurePolicy = CookieSecurePolicy.SameAsRequest
                    };
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                        ValidateIssuer = false,
                        ValidateAudience = false
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

            var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
            services.AddSingleton(emailSettings);

            var ldapSettings = configuration.GetSection("LdapSettings").Get<LdapSettings>();
            services.AddSingleton(ldapSettings);
        }

        private static void InjectServices(IServiceCollection services)
        {
            services.AddScoped<ISessionService, SessionService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IBaseService, BaseService>();

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ILogServiceProvider, LogServiceProvider>();
            services.AddSingleton<IEmployeeService, EmployeeService>();
            services.AddSingleton<ICourseAndCertificateService, CourseAndCertificateService>();
            services.AddSingleton<ITeamService, TeamService>();
            services.AddSingleton<IRequestService, RequestService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<ILateInEarlyOutService, LateInEarlyOutService>();
            services.AddSingleton<IRedmineService, RedmineService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<INexbotService, NexbotService>();
            services.AddSingleton<IJobTitleService, JobTitleService>();
            services.AddSingleton<IShiftService, ShiftService>();
            services.AddSingleton<IHistoryWorkService, HistoryWorkService>();
            services.AddSingleton<IMailEmployeeService, MailEmployeeService>();
            services.AddSingleton<ILeaveTypeService, LeaveTypeService>();
            services.AddSingleton<IToeicService, ToeicService>();
            services.AddSingleton<IWorkFromHomeRequest, WorkFromHomeRequestService>();
            services.AddSingleton<ICalendarService, CalendarService>();
            services.AddSingleton<IOvertimeService, OvertimeService>();
            services.AddSingleton<IScheduledMessageService, ScheduledMessageService>();
            services.AddSingleton<IHolidayService, HolidayService>();
            services.AddSingleton<IAnnualLeaveService, AnnualLeaveService>();
            services.AddSingleton<IPostnatalService, PostnatalService>();
            services.AddSingleton<IEmployeeWorkFromHomeService, EmployeeWorkFromHomeService>();
            services.AddSingleton<IDepartmentService, DepartmentService>();
            services.AddSingleton<IExtensionNoService, ExtensionNoService>();
            services.AddSingleton<IBioService, BioService>();
            services.AddSingleton<ILookupDataService, LookupDataService>();
            services.AddSingleton<IMemoService, MemoService>();
            services.AddSingleton<ICompanyEventService, CompanyEventService>();
            //API Service
            services.AddSingleton<IBotBioTasksService, BioTasksService>();
            services.AddSingleton<IEmployeeOnlineService, EmployeeOnlineService>();
            services.AddSingleton<IBotFogotLogtimeReminderService, FogotLogtimeReminderService>();
            services.AddSingleton<IBotLateInLateOutAutoJobService, BotLateInEarlyOutViolateOnMonthWarningSerivce>();
            services.AddSingleton<IBotAutoJobService, BotMissingCheckInCheckOutReminderSerivce>();
            services.AddSingleton<IMailBirthdayAndFirstWorkingdayReminderService, MailBirthdayAndFirstWorkingdayReminderService>();
            services.AddSingleton<ISyncDataService, SyncDataService>();
            services.AddSingleton<ITimeSheetService, TimeSheetService>();
            services.AddSingleton<IMessageCategoryLogService, MessageCategoryLogService>();
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
            services.AddSingleton<ICalendarRepository, CalendarRepository>();
            services.AddSingleton<IOvertimeRepository, OvertimeRepository>();
            services.AddSingleton<IScheduledMessageRepository, ScheduledMessageRepository>();
            services.AddSingleton<IAnnualLeaveRepository, AnnualLeaveRepository>();
            services.AddSingleton<IBioRepository, BioRepository>();
            services.AddSingleton<IHistoryWorkRepository, HistoryWorkRepository>();
            services.AddSingleton<IDepartmentRepository, DepartmentRepository>();
        }
    }
}