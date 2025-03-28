﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public static class Common
    {

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
            public const string IsAppuserTimeZone = "IsAppuserTimeZone";
            public const string conUserName = "conUserName";
            public const string conPassword = "conPassword";
            public const string Culture = "Culture";



            public const string UserName = "UserName";
            public const string Password = "Password";
            public const string Guid = "Guid";
            public const string Token = "Token";
            public const string OrgDetails = "OrgDetails";

            public const string UserID = "UserID";
            public const string UserDisplayName = "UserDisplayName";
            public const string UserRoleID = "UserRoleID";
            public const string UserRoleName = "UserRoleName";
            public const string UserRoleType = "UserRoleType";
            public const string DisplayPDPA = "DisplayPDPA";
            public const string PayrollAccessible = "PayrollAccessible";
            public const string UserAccessRightsCount = "UserAccessRightsCount";
            public const string SetPassword = "SetPassword";
            public const string PasswordExpiry = "PasswordExpiry";
            public const string ValidateOTP = "ValidateOTP";
            public const string IsProfileUser = "IsProfileUser";
            public const string IsSystemUser = "IsSystemUser";
            public const string UserImgPath = "UserImgPath";
            public const string LanguageCode = "LanguageCode";
            public const string ServiceURL = "ServiceURL";
            public const string RegionCode = "RegionCode";
        }

        public class Messages
        {
            public const string Login = "Try To Login";
            public const string NoRecordsFound = "No Records Found";
            public const string InvalidData = "Invalid Data";
        }
        public class PageMode
        {
            public const string EDIT = "EDIT";
            public const string ADD = "ADD";
            public const string DELETE = "DELETE";
            public const string GET = "GET";
            public const string VIEW ="VIEW";
            public const string USER_GROUP = "USER_GROUP";
            public const string GET_MASTER_LANGUAGE = "GET_MASTER_LANGUAGE";
            public const string USER_ROLE= "USER_ROLE";
            public const string GET_DETAIL= "GET_DETAIL";
            public const string GET_ORG= "GET_ORG";
            public const string GET_TIMEZONE= "GET_TIMEZONE";
            public const string DELETE_ROLE= "DELETE_ROLE";
            public const string UNLOCK= "UNLOCK";
            public const string ADD_ROLE= "ADD_ROLE";
            public const string UPDATE_USER_PASSWORD= "UPDATE_USER_PASSWORD";
            public const string RESET_PWD_MASTER= "RESET_PWD_MASTER";
            public const string ADD_CLIENT_TO_MASTER= "ADD_CLIENT_TO_MASTER";
            public const string GET_USER_ID = "GET_USER_ID";
            public const string EDIT_VALIDATE_USER = "EDIT_VALIDATE_USER";
            public const string GET_DATALOCATION_DROPDOWN= "GET_DATALOCATION_DROPDOWN";
            public const string GET_MODULE_INFORMATION= "GET_MODULE_INFORMATION";
            public const string GET_ORGMODULES= "GET_ORGMODULES";
            public const string MODULES = "MODULES";
            public const string GET_PARENT= "GET_PARENT";
            public const string GET_CLIENT_DETAIL = "GET_CLIENT_DETAIL";
            public const string CHECK_ModuleType = "CHECK_ModuleType";
            public const string CHECKUSEREXISTS = "CHECKUSEREXISTS";
        }
    }

    public static class TableVariables
    {
        public class Organisation
        {
            public const string ID = "ID";
            public const string DBName = "DBName";
            public const string InstanceName = "InstanceName";
            public const string conUserName = "conUserName";
            public const string conPassword = "conPassword";
            public const string Guid = "Guid";
        }
    }
    
}
