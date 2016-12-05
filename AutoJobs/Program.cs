namespace AutoJobs
{
    using System;
    using System.ServiceProcess;

    public static class Program
    {
        public static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[] 
            { 
                new JobService() 
            };
            ServiceBase.Run(servicesToRun);

            ////log4net.Config.XmlConfigurator.Configure();

            ////Fanex.Data.Configuration.DbSettingProviderManager.Current.Start();

            ////MailReminderService service = new MailReminderService();

            ////var today = DateTime.Now;

            ////service.ReminderForgotCheckInOut(today);

            ////////service.ReminderBirthdayAndFirstWorkingday(today);

            ////Console.ReadKey();
        }
    }
}
