namespace Fanex.NexHR.Repositories
{
    using Criteria.Overtime;
    using Data;
    using Fanex.Common;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.ViewModels;
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class OvertimeRepository : IOvertimeRepository
    {
        public IEnumerable<Overtime> ReportDetail(ReportDetailCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = db.Query<Overtime, Employee, Employee, Overtime>(
                    (o, e1, e2) =>
                    {
                        o.CreatedBy = e1;
                        o.Employee = e2;
                        return o;
                    },
                    "CreatedId, EmployeeId",
                    criteria.GetSpParams());

                return result;
            }
        }

        public IEnumerable<Overtime> ReportSummary(ReportSummaryCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = db.Query<Overtime, Employee, Overtime>(
                    (o, e1) =>
                    {
                        o.CreatedBy = e1;
                        return o;
                    },
                    "CreatedId",
                    criteria.GetSpParams());

                return result;
            }
        }

        public OvertimeCreateViewModel Get(GetByIdCriteria criteria)
        {
            var model = new OvertimeCreateViewModel();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var dataset = db.ExecuteDataSet(criteria.GetSpParams());
                model = Convert(dataset);
            }

            return model;
        }

        private OvertimeCreateViewModel Convert(DataSet data)
        {
            var viewModel = new OvertimeCreateViewModel();
            if (data.Tables.Count == 2)
            {
                var overtime = data.Tables[0];
                foreach (DataRow row in overtime.Rows)
                {
                    viewModel.Date = (DateTime)row["Date"];
                    viewModel.Description = row["Description"].ToString();
                    viewModel.Id = (int)row["Id"];
                    viewModel.IsApprove = ((RequestStatuse)row["Status"]) == RequestStatuse.Approve;
                }

                var overtimeDetail = data.Tables[1];
                viewModel.TimeSheets = (from DataRow row in overtimeDetail.Rows
                                        select new OvertimeRecord
                                        {
                                            EmployeeId = (int)row["EmployeeId"],
                                            Hour = Converter.To<float>(row["Hour"]),
                                            DisplayName = row["DisplayName"].ToString()
                                        }).ToList();
            }

            return viewModel;
        }
    }
}