using System;
using Fanex.Data.Repository;

namespace Fanex.NexHR.Criteria.Employee;

public class GetEmployeeConversationCriteria : CriteriaBase
{
    public string EmployeeJson { get; set; }

    public DateTime OnDate { get; set; }

    public override string GetSettingKey() => "EmployeeConversation_ByAvailableList_Get";

    public override bool IsValid() => true;
}