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

        CommandResult AddNewExtensionNoMember(AddNewExtensionNoMemberCriteria criteria);

        ExtensionNoModel GetEmployeeExtensionNoById(GetEmployeeExtensionNoByIdCriteria criteria);

        CommandResult UpdateExtensionNoMember(UpdateExtensionNoMemberCriteria criteria);

        CommandResult DeleteExtensionNumber(DeleteExtensionNumberCriteria criteria);

        CommandResult AddNewExternalGroup(AddNewExternalGroupCriteria criteria);

        CommandResult UpdateExternalGroup(UpdateExternalGroupCriteria criteria);

        CommandResult DeleteExternalGroup(DeleteExternalGroupCriteria criteria);
    }
}
