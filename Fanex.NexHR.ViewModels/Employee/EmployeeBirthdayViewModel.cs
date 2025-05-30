namespace Fanex.NexHR.ViewModels.Employee;

using System.ComponentModel.DataAnnotations;

public class EmployeeBirthdayViewModel
{
    [Display(Name = "Month")]
    public int Month { get; set; }

    [Display(Name = "DoB")]
    public string DateOfBirth { get; set; }

    [Display(Name = "Name")]
    public string FullName { get; set; }

    [Display(Name = "Nick")]
    public string NickName { get; set; }
}