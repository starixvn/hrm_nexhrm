namespace Fanex.NexHR.Criteria.Overtime
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Fanex.NexHR.Core.XML;
    using Models.Overtime;

    public class VerifyOvertimeCriteria : BaseCommand
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

        public override string GetSettingKey()
        {
            return "Overtime_VerifyRequest";
        }

        public override bool IsValid()
        {
            return TimeSheets.Any() && IsValidOvertimeDetail() && Id > 0 && ModifiedBy > 0;
        }

        private bool IsValidOvertimeDetail()
        {
            bool isValid = true;
            int i = 0;

            while (isValid && i < TimeSheets.Count)
            {
                var isValidEmployee = TimeSheets[i].Employee == null || TimeSheets[i].Employee.Id <= 0;

                if (isValidEmployee || TimeSheets[i].NexHour < 0)
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
                    EmployeeId = record.Employee.Id,
                    Date = record.Date,
                    NexHour = record.NexHour
                });
            }

            return result;
        }
    }
}