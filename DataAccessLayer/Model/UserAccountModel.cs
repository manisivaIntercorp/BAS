using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class UserAccountModel
    {
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
        public string? PlatformUser { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? Vendor { get; set; }
        public string? DisplayName { get; set; }
        public string? LanguageID { get; set; }
        public string? CountryID { get; set; }
        public string? TimeZoneID { get; set; }
        public string? emailID { get; set; }
        public string? ContactNo { get; set; }
        public int RoleID { get; set; }
        public string? UserPolicy { get; set; }
        public string? PasswordChange { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public string? AccountLocked { get; set; }
        public int Tenant {  get; set; }
        public string? Active {  get; set; }
        public string ? TempDeactive {  get; set; }
        
        public string? SystemUser { get; set; }
        public string? ProfileUser { get; set; }
        public int  CreatedBy { get; set; }
        public int ProfileID { get;  set; }
    }
}
