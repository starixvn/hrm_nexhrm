// --------------------------------------------------------------------------
// <copyright file="ShiftController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System;
    using System.Web.Http;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// Shift controller.
    /// </summary>
    public class ShiftController : ApiController
    {
        private readonly IShiftService _shiftService;

        public ShiftController() : this(null)
        {
        }

        public ShiftController(IShiftService shiftService = null)
        {
            _shiftService = shiftService ?? new ShiftService();
        }

        /// <summary>
        /// Method get Shift by date.
        /// </summary>
        /// <param name="employeeId">Employee id.</param>
        /// <param name="date">Shift for specific date.</param>
        /// <returns>Shift entity.</returns>
        public Shift GetShiftByDate(int employeeId, DateTime date)
        {
            Shift shiftByDate = new Shift();
            var getShiftByDateCriteria = new GetShiftByDateCriteria()
            {
                EmployeeId = employeeId,
                Date = date
            };
            shiftByDate = _shiftService.GetByDate(getShiftByDateCriteria);

            return shiftByDate;
        }
    }
}
