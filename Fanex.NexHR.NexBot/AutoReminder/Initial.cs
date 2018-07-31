namespace Fanex.NexHR.NexBot
{
    using AutoReminder.BioStation;
    using AutoReminder.PolicyRemind;
    using Hangfire;
    using Hangfire.Storage;
    using Interfaces;
    using WebSite.AutoReminder.PolicyRemind;

    public class Initial
    {
        private readonly INexbotEvent bioDeviceEvent;
        private readonly INexbotEvent timeChangeEvent;
        private readonly INexbotEvent fingerScannerEvent;
        private readonly INexbotEvent missCheckInOutEvent;
        private readonly INexbotEvent forgotLogtimeEvent;

        public Initial()
        {
            bioDeviceEvent = new BioDeviceMonitor();
            timeChangeEvent = new BioTimeChangeMonitor();
            fingerScannerEvent = new BioFingerScannerMonitor();
            missCheckInOutEvent = new MissCheckInCheckOutRemind();
            forgotLogtimeEvent = new ForgotSpendTimeRemind();
        }

        public void Initialize()
        {
            RemoveAllRecurringJob();
            InitAllEvent();
        }

        private static void RemoveAllRecurringJob()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }
        }

        private void InitAllEvent()
        {
            bioDeviceEvent.Initialize();
            timeChangeEvent.Initialize();
            fingerScannerEvent.Initialize();
            missCheckInOutEvent.Initialize();
            forgotLogtimeEvent.Initialize();
        }
    }
}