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
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Models;

    public interface IExtensionNoRepository
    {
        IList<ExtensionNoModel> GetAllExtensionNo(ICriteria criteria);

        IList<ExtensionNoModel> GetAllExtensionGroupInfo(ICriteria criteria);

        bool AddNewExtensionNoMember(ICriteria criteria, out int errorCode, out string message);

        ExtensionNoModel GetEmployeeExtensionNoById(ICriteria criteria);

        bool UpdateExtensionNoMember(ICriteria criteria, out int errorCode, out string message);

        bool DeleteExtensionNumber(ICriteria criteria, out int errorCode, out string message);

        bool AddNewExternalGroup(ICriteria criteria, out int errorCode, out string message);

        bool UpdateExternalGroup(ICriteria criteria, out int errorCode, out string message);

        bool DeleteExternalGroup(ICriteria criteria, out int errorCode, out string message);
    }
}
