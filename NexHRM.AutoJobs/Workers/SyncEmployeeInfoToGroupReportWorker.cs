namespace NexHRM.AutoJobs.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexHRM.AutoJobs.Constants;
    using NexHRM.AutoJobs.Helpers;
    using NexHRM.AutoJobs.Models;
    using NexHRM.AutoJobs.Services;
    using Microsoft.Extensions.Logging;

    public class SyncEmployeeInfoToGroupReportWorker : BaseWorker
    {
        private readonly IGroupReportService groupReportService;

        public SyncEmployeeInfoToGroupReportWorker(
            IGroupReportService groupReportService,
            List<JobSetting> jobSettings,
            ILogger<SyncEmployeeInfoToGroupReportWorker> logfile,
            Fanex.Logging.ILogger logger) : base(logfile, logger)
        {
            this.groupReportService = groupReportService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobSettingConstants.SyncEmployeeInfoToGroupReport);
        }

        public override async Task DoWorkAsync()
        {
            await groupReportService.SyncEmployeeInfoAsync();
        }
    }
}