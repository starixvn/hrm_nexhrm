using System;

namespace Fanex.NexHR.Models;

public class EmployeeMealModel
{
    public int EmpId { get; set; }

    public int SysId { get; set; }

    public string NickName { get; set; }

    public int CompanyId { get; set; }

    public string ConversationId { get; set; }

    public DateTime OnDate { get; set; }

    public int MealId { get; set; } = 2; // Default to 2 (Lunch)
}