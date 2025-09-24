namespace Fanex.NexHR.Criteria.Auth
{
    public class ChangePasswordCommand : BaseCommand
    {
        public int Id { get; set; }

        public string Password { get; set; }

        public override string GetSettingKey() => "EmployeeExt_ChangePassword";

        public override bool IsValid() => Id > 0 && !string.IsNullOrEmpty(Password);
    }
}