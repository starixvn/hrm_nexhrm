namespace Fanex.NexHR.WebSite
{
    using System;
    using System.Text;
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
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
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

            var chatbotConfigs = configuration.GetSection("ChatbotConfigs").Get<ChatbotConfigs>();
            services.AddSingleton(chatbotConfigs);

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
            services.AddSingleton<Logging.ILogger, Logging.Logger>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IBaseService, BaseService>();

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ILogServiceProvider, LogServiceProvider>();
            services.AddSingleton<IEmployeeService, EmployeeService>();
            services.AddSingleton<ITrainingAndDevelopmentService, TrainingAndDevelopmentService>();
            services.AddSingleton<IPromotionService, PromotionService>();
            services.AddSingleton<ITeamService, TeamService>();
            services.AddSingleton<IRequestService, RequestService>();
            services.AddSingleton<ILateInEarlyOutService, LateInEarlyOutService>();
            services.AddSingleton<IRedmineService, RedmineService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<INexbotService, NexbotService>();
            services.AddSingleton<IJobTitleService, JobTitleService>();
            services.AddSingleton<IShiftService, ShiftService>();
            services.AddSingleton<IMailEmployeeService, MailEmployeeService>();
            services.AddSingleton<ILeaveTypeService, LeaveTypeService>();
            services.AddSingleton<IToeicService, ToeicService>();
            services.AddSingleton<ICalendarService, CalendarService>();
            services.AddSingleton<IOvertimeService, OvertimeService>();
            services.AddSingleton<IScheduledMessageService, ScheduledMessageService>();
            services.AddSingleton<IHolidayService, HolidayService>();
            services.AddSingleton<IPostnatalService, PostnatalService>();
            services.AddSingleton<IDepartmentService, DepartmentService>();
            services.AddSingleton<IExtensionNoService, ExtensionNoService>();
            services.AddSingleton<IBioService, BioService>();
            services.AddSingleton<ILookupDataService, LookupDataService>();
            services.AddSingleton<IMemoService, MemoService>();
            services.AddSingleton<IApplicantService, ApplicantService>();
            services.AddSingleton<IHistoryService, HistoryService>();
            services.AddSingleton<IPositionService, PositionService>();
            services.AddSingleton<ISupervisorFlowService, SupervisorFlowService>();
            services.AddSingleton<ICryptoService, CryptoService>(); 

            //API Service
            services.AddSingleton<IBotBioTasksService, BioTasksService>();
            services.AddSingleton<IBotFogotLogtimeReminderService, FogotLogtimeReminderService>();
            services.AddSingleton<IBotLateInLateOutAutoJobService, BotLateInEarlyOutViolateOnMonthWarningSerivce>();
            services.AddSingleton<IBotAutoJobService, BotMissingCheckInCheckOutReminderSerivce>();
            services.AddSingleton<IBotLogTimeRemindService, BotLogTimeRemindService>();
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
            services.AddSingleton<IRequestRepository, RequestRepository>();
            services.AddSingleton<IReportRepository, ReportRepository>();
        }
    }
}