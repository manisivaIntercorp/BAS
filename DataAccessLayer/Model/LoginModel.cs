using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{

    public class LoginDetailModel
    {
        public  int? UserID { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }
        public string? TwoFAAuthentication { get; set; }
        public string? PromptPasswordChange { get; set; }
        public string? PasswordExpiry { get; set; }
        public string? PasswordExpiryDate { get; set; }
        public int? RoleID { get; set; }
        public string? RoleDesc { get; set; }
        public string? RoleName { get; set; }
        public string? RoleType { get; set; }
        public int? AccessRightsCount { get; set; }
        public string? DisplayPDPAData { get; set; }
        public string? IsPayrollAccessible { get; set; }
        public string? DBName { get; set; }
        public string? LanguageCode { get; set; }
        public string? TimeOffset { get; set; }
        public string? TimeZoneID { get; set; }

        public int? TimeZone { get; set; }
        public string? IsAppuserTimeZone { get; set; }

        public string? IsProfileUser { get; set; }
        public string? IsSystemUser { get; set; }
        public string? ImagePath { get; set; }
        public string? DisplayPDPA { get; set; }
        public string? PayrollAccessible { get; set; }
        public string? UserAccessRightsCount { get; set; }
        public string? SetPassword { get; set; }
        public string? ValidateOTP { get; set; }


    }


    public class LoginModel
    {
        public string? Mode { get; set; }
        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? UserType { get; set; }

        public string? IPAddress { get; set; }

        public string? DeviceName { get; set; }

        public string? BrowserName { get; set; }
        public string? GlobalUser { get; set; }
        public string? LanguageCode { get; set; }

        

    }
    public class ResultModel
    {
        public int? RetVal { get; set; }
        public  string? RetMsg { get; set; }

        public List<LoginDetailModel>? lstLoginDetails { get; set;}
    }

    public class OrganisationDBDetails
    {
        public int? ID { get; set; }
        public string? OrgCode { get; set; }
        public string? OrgName { get; set; }
        public string? Logo { get; set; }
        public string? DBName { get; set; }
        public string? AutoID { get; set; }
        public string? InstanceName { get; set; }
        public string? ConUserName { get; set; }
        public string? ConPassword { get; set; }
    }
    public class GetDropDownDataModel
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
}
