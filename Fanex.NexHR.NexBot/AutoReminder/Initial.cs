namespace Fanex.NexHR.NexBot
{
    using AutoReminder.BioStation;
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

        public Initial()
        {
            bioDeviceEvent = new BioDeviceMonitor();
            timeChangeEvent = new BioTimeChangeMonitor();
            fingerScannerEvent = new BioFingerScannerMonitor();
            missCheckInOutEvent = new MissCheckInCheckOutRemind();
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
        }
    }
}