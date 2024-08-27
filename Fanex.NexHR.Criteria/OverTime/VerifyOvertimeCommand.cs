namespace Fanex.NexHR.Criteria.Overtime
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Fanex.NexHR.Core.XML;
    using Models.Overtime;

    public class VerifyOvertimeCommand : BaseCommand
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IList<OvertimeRecord> TimeSheets { get; set; }

        public string OvertimeSheet
        {
            get
            {
                return TimeSheetsXml().ToXML().ToString();
            }
        }

        public override string GetSettingKey() => "Overtime_VerifyRequest";

        public override bool IsValid() => TimeSheets.Any() && IsValidOvertimeDetail() && Id > 0 && ModifiedBy > 0;

        private bool IsValidOvertimeDetail()
        {
            bool isValid = true;
            int i = 0;

            while (isValid && i < TimeSheets.Count)
            {
                var isInValidEmployee = TimeSheets[i].EmployeeId <= 0;

                if (isInValidEmployee || TimeSheets[i].NexHour < 0)
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
                    NexHour = record.NexHour
                });
            }

            return result;
        }
    }
}