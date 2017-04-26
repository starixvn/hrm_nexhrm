namespace Fanex.NexHR.Web
{
    using Core.Enums;
    using System.Web.Security;

    public static class Utilities
    {
        public static bool UserHasOnlyHRRole()
        {
            return Roles.GetRolesForUser().Length == 2 && Roles.IsUserInRole(Role.HR.ToString());
        }
    }
}
