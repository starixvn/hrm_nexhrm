// --------------------------------------------------------------------------
// <copyright file="TOEICService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.05.31">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interface;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// TOEIC service.
    /// </summary>
    public class TOEICService : ITOEICService
    {
        private readonly ITOEICRepository _toeicRepository;

        public TOEICService(ITOEICRepository toeicRepository = null)
        {
            this._toeicRepository = toeicRepository ?? new TOEICRepository();
        }

        /// <summary>
        /// Get TOEIC of specific employee.
        /// </summary>
        /// <param name="empId">Id of employee.</param>
        /// <returns>TOEIC object.</returns>
        public TOEIC GetById(int empId)
        {
            return this._toeicRepository.GetById(empId);
        }

        /// <summary>
        /// Update new TOEIC score.
        /// </summary>
        /// <param name="model">TOEIC object.</param>
        /// <returns>Effected row.</returns>
        public int Update(TOEIC model)
        {
            return this._toeicRepository.Update(model);
        }

        /// <summary>
        /// Update new TOEIC score.
        /// </summary>
        /// <param name="model">TOEIC object.</param>
        /// <returns>Effected row.</returns>
        public int NewUpdate(TOEIC model)
        {
            return this._toeicRepository.NewUpdate(model);
        }
    }
}
