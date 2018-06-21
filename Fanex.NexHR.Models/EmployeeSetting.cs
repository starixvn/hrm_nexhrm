namespace Fanex.NexHR.Models
{
    public class EmployeeSetting
    {
        public int EmpId { get; set; }

        public bool DisplayInfoAndTodoList { get; set; }

        public bool DisplayTeamAttendence { get; set; }

        public bool DisplayTeamWorkload { get; set; }

        public bool DisplayRedmineTask { get; set; }

        public bool DisplayRedmineInputSpendTime { get; set; }
    }
}