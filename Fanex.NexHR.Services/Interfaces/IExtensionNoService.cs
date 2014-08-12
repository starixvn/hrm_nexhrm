// --------------------------------------------------------------------------
// <copyright file="IExtensionNoService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.3.14">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Models;

    public interface IExtensionNoService
    {
        IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNoCriteria criteria);

        IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetAllExtensionGroupInfoCriteria criteria);

        bool AddNewExtensionNoMember(AddNewExtensionNoMemberCriteria criteria);

        ExtensionNoModel GetEmployeeExtensionNoById(GetEmployeeExtensionNoByIdCriteria criteria);

        bool UpdateExtensionNoMember(UpdateExtensionNoMemberCriteria criteria);

        bool DeleteExtensionNumber(DeleteExtensionNumberCriteria criteria);

        bool AddNewExternalGroup(AddNewExternalGroupCriteria criteria);

        bool UpdateExternalGroup(UpdateExternalGroupCriteria criteria);

        bool DeleteExternalGroup(DeleteExternalGroupCriteria criteria);
    }
}
