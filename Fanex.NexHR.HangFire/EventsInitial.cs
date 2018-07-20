namespace Fanex.NexHR.HangFire
{
    using BioTask;
    using Hangfire;
    using Hangfire.Storage;

    public class EventsInitial
    {
        private readonly INexbotEvent bioDeviceEvent;
        private readonly INexbotEvent timeChangeEvent;
        private readonly INexbotEvent fingerScannerEvent;

        public EventsInitial()
        {
            bioDeviceEvent = new BioDeviceMonitor();
            timeChangeEvent = new BioTimeChangeMonitor();
            fingerScannerEvent = new BioFingerScannerMonitor();
        }

        public void Initialize()
        {
            RemoveAllRecurringJob();
            InitAllEvent();
        }

        private void RemoveAllRecurringJob()
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
        }
    }
}