namespace Fanex.NexHR.Services.AutoJobService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.EmployeeOnline;
    using Fanex.NexHR.Criteria.EmployeeOnlines;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeOnline;
    using Fanex.NexHR.Services.Interfaces;

    public class EmployeeOnlineService : IEmployeeOnlineService
    {
        private readonly string getallComputerNameCommand = "import-module activedirectory\n\r  Get-ADComputer -Filter 'Name -like \"CP*\" -or Name -like \"NB*\"' | Select -Expand Name";
        private readonly string checkConnectionCommand = "Test-Connection -ComputerName {0} -BufferSize 16 -Count 1 -Quiet -ErrorAction SilentlyContinue";

        private readonly IShellCommandService shellCommandService;
        private readonly IBaseService baseService;

        public EmployeeOnlineService(IShellCommandService shellCommandService, IBaseService baseService)
        {
            this.shellCommandService = shellCommandService;
            this.baseService = baseService;
        }

        public async Task RunCheckOnline()
        {
            var criteria = new GetEmployeeOnlineStatusCriteria();

            var employees = baseService.GetData<Employee>(criteria);

            var computers = GetComputerNames();

            var employeeOnlines = new List<EmployeeOnline>();

            foreach (var computer in computers)
            {
                var computerArr = computer.Split('-');
                if (computerArr.Count() > 1)
                {
                    var nick = computerArr[1];
                    if (!employeeOnlines.Any(t => t.NickName.Equals(nick)) && employees.Any(t => t.NickName.Equals(nick, StringComparison.OrdinalIgnoreCase)))
                    {
                        var isOnline = CheckIsOnline(computer);
                        var employee = employees.FirstOrDefault(t => t.NickName.Equals(nick, StringComparison.OrdinalIgnoreCase));

                        employeeOnlines.Add(
                                new EmployeeOnline
                                {
                                    EmpId = employee.Id,
                                    NickName = employee.NickName,
                                    IsOnline = isOnline,
                                    CheckingTime = DateTime.Now,
                                    PCName = computer
                                }
                              );
                    }
                }
            }

            await UpdateEmployeeOnline(employeeOnlines);
        }

        private async Task UpdateEmployeeOnline(IList<EmployeeOnline> employeeOnlines)
        {
            var criteria = new UpdateEmployeeOnlineCommand
            {
                EmployeeOnlines = employeeOnlines
            };

            await baseService.ExecuteNonQueryAsync(criteria);
        }

        private string[] GetComputerNames()
        {
            var computerText = shellCommandService.ExcuteCommand(getallComputerNameCommand);

            var computers = computerText.Split(new[] { "\r\n" }, StringSplitOptions.None);

            return computers;
        }

        private bool CheckIsOnline(string pcName)
        {
            var output = shellCommandService.ExcuteCommand(string.Format(checkConnectionCommand, pcName));
            return output.Contains("True");
        }
    }
}