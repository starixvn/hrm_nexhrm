namespace AutoJobs
{
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;
    using log4net;
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Timers;

    public partial class JobService : ServiceBase
    {
        private const int DefaultTimerInterval = 300;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly MailReminderService MailReminderService = new MailReminderService();

        private readonly Timer _updateCounterTimer;
        private bool _isBusy;

        public JobService()
        {
            InitializeComponent();
            DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider(
                    resourcePath: "~/App_Data",
                    enableWatching: true)
                .Run();
            const int timeLine = 1000;
            int timerInterval = DefaultTimerInterval;

            int configTimerInterval = int.Parse(ConfigurationManager.AppSettings["TimerInterval"]);
            if (configTimerInterval > -1)
            {
                timerInterval = configTimerInterval;
            }

            TimerInterval = timerInterval;
            _updateCounterTimer = new Timer(TimerInterval * timeLine);
            _updateCounterTimer.Elapsed += ExecuteJob;
            _updateCounterTimer.Enabled = false;
        }

        public int TimerInterval { get; }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Reviewed. Suppression is OK here.")]
        public void ExecuteJob(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!_isBusy)
                {
                    _isBusy = true;
                    var today = DateTime.Now;

                    //MailReminderService.ReminderForgotCheckInOut(today);

                    //MailReminderService.ReminderBirthdayAndFirstWorkingday(today);

                    _isBusy = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error: " + ex.Message);
            }
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("Service started at " + DateTime.Now);

            _updateCounterTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            _updateCounterTimer.Enabled = false;

            Log.Info("Service stopped at " + DateTime.Now);
        }
    }
}
