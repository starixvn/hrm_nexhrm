// --------------------------------------------------------------------------
// <copyright file="TOEICService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.05.31">Created</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services
{
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Toeic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// TOEIC service.
    /// </summary>
    public class ToeicService : IToeicService
    {
        private readonly IToeicRepository _toeicRepository;

        public ToeicService(IToeicRepository toeicRepository = null)
        {
            this._toeicRepository = toeicRepository ?? new ToeicRepository();
        }

        /// <summary>
        /// Get TOEIC of specific employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>TOEIC object.</returns>
        public Toeic GetById(GetToeicScoreByEmployeeIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Toeic toeic = null;
            if (criteria.IsValid())
            {
                toeic = this._toeicRepository.GetById(criteria);
            }

            return toeic;
        }

        /// <summary>
        /// Update new TOEIC score.
        /// </summary>
        /// <param name="criteria">UpdateTOEICScore Criteria.</param>
        /// <returns>Effected row.</returns>
        public int UpdateToeicScore(UpdateToeicScoreCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int updated = -1;
            if (criteria.IsValid())
            {
                updated = this._toeicRepository.Update(criteria);
            }

            return updated;
        }

        /// <summary>
        /// Update Full TOEIC Score.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Effected row.</returns>
        public int UpdateFullToeicScore(UpdateFullToeicScoreCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int updated = -1;
            if (criteria.IsValid())
            {
                updated = this._toeicRepository.Update(criteria);
            }

            return updated;
        }
    }
}
