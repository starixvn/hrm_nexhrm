// --------------------------------------------------------------------------
// <copyright file="ExportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;

    /// <summary>
    /// Export controller.
    /// </summary>
    public class ExportController : BaseController
    {
        /// <summary>
        /// Export total working hours.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        public ActionResult TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            var _reportRepository = new ReportRepository();
            var _reportService = new EmployeeService();
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            if (model == null)
            {
                model = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = 5,
                };
            }

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            string fileName = "TotalWorkingHours_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xls";

            IList<TotalWorkingHours> listTotalWorkingHours = _reportRepository.GetWorkingHoursList(
                model.FromDate,
                model.ToDate,
                empId: (model.EmployeeId != -1 & model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                groupId: model.TeamId);

            _reportService.ExportTotalWorkingHours(hssfworkbook, listTotalWorkingHours, model.FromDate, model.ToDate);

            var output = new MemoryStream();
            hssfworkbook.Write(output);

            // Return the result to the end user
            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }
    }
}
