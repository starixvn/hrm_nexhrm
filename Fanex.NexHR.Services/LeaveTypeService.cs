// --------------------------------------------------------------------------
// <copyright file="LeaveTypeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.LeaveType;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Leave type service.
    /// </summary>
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly ILeaveTypeRepository _leaveTypeRepository;

        public LeaveTypeService(ILeaveTypeRepository leaveTypeRepository = null)
        {
            this._leaveTypeRepository = leaveTypeRepository ?? new LeaveTypeRepository();
        }

        public IList<LeaveType> GetAll()
        {
            var criteira = new GetLeaveTypeCriteria();
            return this._leaveTypeRepository.GetAll(criteira);
        }
    }
}
