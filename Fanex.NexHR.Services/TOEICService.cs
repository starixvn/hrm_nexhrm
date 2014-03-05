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
    using Fanex.NAP.Framework.Data;
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
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>TOEIC object.</returns>
        public TOEIC GetById(ICriteria criteria)
        {
            var toeic = new TOEIC();
            if (criteria.IsValid())
            {
                toeic = this._toeicRepository.GetById(criteria);
            }

            return toeic;
        }

        /// <summary>
        /// Update new TOEIC score.
        /// </summary>
        /// <param name="criteria">UpdateFullTOEICScore Criteria.</param>
        /// <returns>Effected row.</returns>
        public int Update(ICriteria criteria)
        {
            int updated = -1;
            if (criteria.IsValid())
            {
                updated = this._toeicRepository.Update(criteria);
            }

            return updated;
        }
    }
}
