using System.Collections.Generic;

namespace Infrastructure.Session
{
    public class SessionConstants
    {
        public const string SessionEntity = "SessionEntity";
        public const string SessionLogId = "SessionLogId";
        public const string IsDebug = "IsDebug";

        public const string UserEntity = "UserEntity";
        public const string UserPermissions = "UserPermissions";

        public static List<string> ExcludedSessionPaths
        {
            get
            {
                return new List<string>()
                {
                    "/Diagnostics/",
                    "/Email/",
                    "/Session/"
                };
            }
        }
    }
}
