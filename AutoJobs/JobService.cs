namespace AutoJobs
{
    using log4net;
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Timers;

    public partial class JobService : ServiceBase
    {
        private const int _DEFAULT_TIMER_INTERVAL = 300;
        private static readonly ILog _LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly MailReminderService mailReminderService = new MailReminderService();

        private Timer _updateCounterTimer = null;
        private bool _isBusy = false;

        public JobService()
        {
            InitializeComponent();
            Fanex.Data.Configuration.DbSettingProviderManager.Current.Start();
            int timerInterval = _DEFAULT_TIMER_INTERVAL;

            int configTimerInterval = int.Parse(ConfigurationManager.AppSettings["TimerInterval"]);
            if (configTimerInterval > -1)
            {
                timerInterval = configTimerInterval;
            }

            TimerInterval = timerInterval;
            _updateCounterTimer = new Timer(TimerInterval * 1000);
            _updateCounterTimer.Elapsed += new ElapsedEventHandler(ExecuteJob);
            _updateCounterTimer.Enabled = false;
        }

        public int TimerInterval { get; private set; }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Reviewed. Suppression is OK here.")]
        public void ExecuteJob(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!_isBusy)
                {
                    _isBusy = true;
                    var today = DateTime.Now;

                    mailReminderService.ReminderForgotCheckInOut(today);

                    mailReminderService.ReminderBirthdayAndFirstWorkingday(today);

                    _isBusy = false;
                }
            }
            catch (Exception ex)
            {
                _LOG.Error("Error: " + ex.Message);
            }
        }

        protected override void OnStart(string[] args)
        {
            _LOG.Info("Service started at " + DateTime.Now.ToString());

            _updateCounterTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            _updateCounterTimer.Enabled = false;

            _LOG.Info("Service stopped at " + DateTime.Now.ToString());
        }
    }
}
