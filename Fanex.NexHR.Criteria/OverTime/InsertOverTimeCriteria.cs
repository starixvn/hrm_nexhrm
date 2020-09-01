namespace Fanex.NexHR.Criteria.Overtime
{
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Data.Repository;
    using Fanex.NexHR.Core.XML;
    using Models.Overtime;

    public class InsertOvertimeCriteria : CriteriaBase
    {
        public int CreatedBy { get; set; }

        public string Description { get; set; }

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
            return "Overtime_Insert";
        }

        public override bool IsValid()
        {
            return TimeSheets.Any() && IsValidOvertimeDetail();
        }

        private bool IsValidOvertimeDetail()
        {
            bool isValid = true;
            int i = 0;

            while (isValid && i < TimeSheets.Count)
            {
                var isValidEmployee = TimeSheets[i].Employee == null || TimeSheets[i].Employee.Id <= 0;
                var isValidHour = TimeSheets[i].Hour <= 0 || TimeSheets[i].Hour > 24 || TimeSheets[i].NexHour < 0;

                if (isValidEmployee || isValidHour)
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
                    Hour = record.Hour,
                    NexHour = record.NexHour,
                    Note = record.Note
                });
            }

            return result;
        }
    }
}
