namespace Fanex.NexHR.Web
{
    using Core.Enums;

    public static class Utilities
    {
        private const int length = 2;

        public static bool UserHasOnlyHRRole()
        {
            return Roles.GetRolesForUser().Length == length && Roles.IsUserInRole(Role.HR.ToString());
        }
    }
}
