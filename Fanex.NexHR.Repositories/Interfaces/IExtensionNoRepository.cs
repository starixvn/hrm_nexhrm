// --------------------------------------------------------------------------
// <copyright file="IExtensionNoRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.3.14">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface IExtensionNoRepository
    {
        IList<ExtensionNoModel> GetAllExtensionNo(ICriteria criteria);

        IList<ExtensionNoModel> GetAllExtensionGroupInfo(ICriteria criteria);

        ExtensionNoModel GetEmployeeExtensionNoById(ICriteria criteria);
    }
}
