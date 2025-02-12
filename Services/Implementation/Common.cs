using WebApi.Services.Interface;

namespace WebApi.Services.Implementation
{
    public class Common : ICommon
    {


        public class TableVariables()
        {
            public class LoginVariable()
            {

                public const string ID = "ID";
            }
        }
        public class SessionVariables()
        {
            public const string DBName = "DBName";
            public const string DBChange = "DBChange";
            public const string InstanceChange = "InstanceChange";
            public const string InstanceName = "InstanceName";
            public const string DataBaseUserName = "DataBaseUserName";
            public const string DataBasePassword = "DataBasePassword";
            public const string GlobalUser = "GlobalUser";
            public const string Language = "Language";
            public const string TimeOffset = "TimeOffset";
            public const string TimeZoneID = "TimeZoneID";
        }
        //public static class SessionVariables()
        //{

        //}
    }
}
