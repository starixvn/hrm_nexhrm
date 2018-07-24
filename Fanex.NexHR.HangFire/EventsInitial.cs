namespace Fanex.NexHR.HangFire
{
    using AutoReminder.PolicyRemind;
    using BioTask;
    using Hangfire;
    using Hangfire.Storage;

    public class EventsInitial
    {
        private readonly INexbotEvent bioDeviceEvent;
        private readonly INexbotEvent timeChangeEvent;
        private readonly INexbotEvent fingerScannerEvent;
        private readonly INexbotEvent missCheckInOutEvent;

        public EventsInitial()
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

            missCheckInOutEvent.Initialize();
        }
    }
}