using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;
using System.Text.Json.Serialization;

namespace DataAccessLayer.Model
{
    public class UserAccountModel
    {
        public DataTable UserAccountOrgTable = new DataTable();
        public DataTable UserAccountRoleTable = new DataTable();
        public DataTable ConvertToDataTable(List<RoleName> models, int id)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(RoleName).GetProperties();
            UserAccountRoleTable.Columns.Add("UserID", typeof(Int64));
            foreach (var property in properties)
            {

                UserAccountRoleTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserAccountRoleTable.NewRow();
                foreach (var property in properties)
                {
                    if (id == 0)
                    {
                        row["UserID"] = 0;
                    }
                    else if (id > 0)
                    {
                        row["UserID"] = id;
                    }
                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                UserAccountRoleTable.Rows.Add(row);
            }

            return UserAccountRoleTable;
        }
        public DataTable ConvertToDataTable(List<UserAccountOrgDatatable> models, int id)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(UserAccountOrgDatatable).GetProperties();
            UserAccountOrgTable.Columns.Add("UserID", typeof(Int64));
            foreach (var property in properties)
            {

                UserAccountOrgTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserAccountOrgTable.NewRow();
                foreach (var property in properties)
                {
                    if (id == 0)
                    {
                        row["UserID"] = 0;
                    }
                    else if (id > 0)
                    {
                        row["UserID"] = id;
                    }
                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                UserAccountOrgTable.Rows.Add(row);
            }

            return UserAccountOrgTable;
        }
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
        public string? PlatformUser { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? Vendor { get; set; }
        public string? DisplayName { get; set; }
        public long LanguageID { get; set; }
        public string? TimeZoneID { get; set; }
        public string? emailID { get; set; }
        public string? ContactNo { get; set; }
        public int RoleID { get; set; }
        public long UserPolicy { get; set; }
        public string? PasswordChange { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public string? AccountLocked { get; set; }
        public int Tenant { get; set; }
        public string? Active { get; set; }
        public string? TempDeactive { get; set; }
        public string? SystemUser { get; set; }
        public string? ProfileUser { get; set; }
        public int CreatedBy { get; set; }
        public int ProfileID { get; set; }
    }
    public class UserAccountOrgDatatable
    {
        public Int64 OrgID { get; set; }
        public DateTime? EffectiveDateOrg { get; set; }
        public string? ActiveOrg { get; set; }
    }
    public class OrgDetails
    {
        public long id { get; set; }
        public string? OrgCode { get; set; }
        public string? OrgName { get; set; }
        public string? DBName { get; set; }
        public string? InstanceName { get; set; }
        public string? ConUserName { get; set; }
        public string? ConPassword { get; set; }
        public string? Selected { get; set; }
    }
    public class UserAccountUpdateRequest
    {
        public UserAccountModel UserAccount { get; set; }
        public List<RoleName> RoleNameList { get; set; } = new List<RoleName>();
        
        public List<UserAccountOrgDatatable> OrgDatatable { get; set; }
        

    }
    public class DeleteRoleinUserAccount
    {
        public DeleteRoleName DeleteRoleNamesingle {  get; set; }
        public List<DeleteRoleNameinList> DeleteRoleName { get; set; }

    }
    public class GetRoleName{
        public long Value { get; set; }
        public string Text { get; set; }
    }
    public class DeleteRoleName
    {
        public DataTable UserAccountDeleteRoleTable = new DataTable();
        public DataTable ConvertToDataTable(List<DeleteRoleNameinList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(DeleteRoleNameinList).GetProperties();
            foreach (var property in properties)
            {
                UserAccountDeleteRoleTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserAccountDeleteRoleTable.NewRow();
                foreach (var property in properties)
                {
                    
                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                UserAccountDeleteRoleTable.Rows.Add(row);
            }

            return UserAccountDeleteRoleTable;
        }
        public long CreatedBy { get; set; }
    }
    public class DeleteRoleNameinList
    {
        public long? id { get; set; }
    }
    public class GetRoleNameDetails
    {
        public long Value { get; set; }
        public string? Text { get; set; }
    }

    public class RoleName
    {
        public long? RoleID { get; set; }
        public DateOnly? RoleNameEffectiveDate { get; set; }
        public long CreatedBy { get; set; }
    }

    public class ResetPassword
    {
        public long? UserId { get; set; }
       public string? UserName { get; set; }
        public string? Password { get; set; }
        public long? CreatedBy { get; set; }
        public long? LevelID { get; set; }
        public long? LevelDetailID { get; set; }
        public string? TimeZoneID { get; internal set; }
    }
    public class UserPolicyName
    {
        public long Value { get; set; }
        public string Text { get; set; }
    }
    public class GetUserAccountRole
    {
        public long? ID {  get; set; }
        public long? RoleID { get; set; }
        public DateTime? EffectiveDate { get;set; }
        public string? AccessToAllClient {  get; set; }

    }
    public class GetUserAccountOrg
    {
        public long? ID { get; set; }
        public string OrgCode {  get; set; }
        public string? Selected {  get; set; }
    }
    public class GetUserAccount
    {
        public long? UserID { get; set; }
        public string UserName { get; set; }
        public string? PromptPasswordChange { get; set; }
        public string? EmailID { get; set; }
        public string? DisplayName { get; set; }
        public string? ContactNo { get; set; }
        public string? UserPolicy { get; set; }
        public string? AccountLocked { get; set; }
        public string? Active { get; set; }
        public DateTime? LastDate { get; set; }
        public string? SystemUser { get; set; }
        public string? ProfileUser { get; set; }
        public long? ProfileID { get; set; }
        public string? UserGroupCode { get; set; } 
        public string? TimeZone {  get; set; }
        public string? LanguageName {  get; set; }
        public long? LanguageID {  get; set; }
        public string? UserImage { get; set; }
    }
    public class UserAccountResponse
    {
        public GetUserAccount User { get; set; }
        public List<GetUserAccountRole> Roles { get; set; }
        public List<GetUserAccountOrg> Organizations { get; set; }
    }
    public class UnlockUser
    {
        public long? UpdatedBy { get; set; }
        public DataTable UnlockTable = new DataTable();
        public DataTable ConvertToDataTable(List<UnlockUserList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(UnlockUserList).GetProperties();
            foreach (var property in properties)
            {

                UnlockTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UnlockTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                UnlockTable.Rows.Add(row);
            }

            return UnlockTable;
        }
    }
    public class UnlockUserList
    {
        public long? UserID { get; set; }
        public string? UserName { get; set; }
    }
    public class ListofUnlock
    {
        public List<UnlockUserList> Users
        {
            get; set;

        }
        public UnlockUser User
        {
            get; set;

        }
    }
    }
