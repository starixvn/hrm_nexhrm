namespace Fanex.NexHR.Criteria.Overtime
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Fanex.NexHR.Core.XML;
    using Models.Overtime;

    public class BaseActionOvertimeCommand : BaseCommand
    {
        public string Description { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<OvertimeRecord> TimeSheets { get; set; }

        public string OvertimeSheet
        {
            get
            {
                return TimeSheetsXml().ToXML().ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "";
        }

        public override bool IsValid()
        {
            return TimeSheets.Any() && IsValidOvertimeDetail();
        }

        private bool IsValidOvertimeDetail()
        {
            bool isValid = true;
            int i = 0;
            var overTimeSheets = TimeSheets.ToList();
            while (isValid && i < TimeSheets.Count())
            {
                var isInValidEmployee = overTimeSheets[i].EmployeeId <= 0;
                var isInValidHour = overTimeSheets[i].Hour <= 0 || overTimeSheets[i].Hour > 24 || overTimeSheets[i].NexHour < 0;

                if (isInValidEmployee || isInValidHour)
                {
                    isValid = false;
                }

                i++;
            }

            return isValid;
        }

        private IList<OvertimeRecordXML> TimeSheetsXml()
        {
            var result = new List<OvertimeRecordXML>();

            foreach (var record in TimeSheets)
            {
                result.Add(new OvertimeRecordXML
                {
                    EmployeeId = record.EmployeeId,
                    Date = record.Date,
                    Hour = record.Hour,
                    NexHour = record.NexHour,
                    Note = record.Note
                });
            }

            return result;
        }
    }
}