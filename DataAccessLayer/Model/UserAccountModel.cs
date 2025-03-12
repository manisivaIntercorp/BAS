using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;

namespace DataAccessLayer.Model
{
    public class UserAccountModel
    {
        public DataTable UserAccountOrgTable = new DataTable();
        public DataTable UserAccountRoleTable = new DataTable();
        public DataTable ConvertToDataTable(List<RoleNameInUserAccount> models, long CreatedBy,int id)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(RoleNameInUserAccount).GetProperties();
            if (UserAccountRoleTable.Columns.Contains("UserID"))
            {
                UserAccountRoleTable.Rows.Clear();
            }
            else
            {
                UserAccountRoleTable.Columns.Add("UserID", typeof(Int64));
            }
            
            foreach (var property in properties)
            {
                if (!UserAccountRoleTable.Columns.Contains(property.Name))
                {
                    UserAccountRoleTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
                    
            }
            if (UserAccountRoleTable.Columns.Contains("CreatedBy"))
            {
                UserAccountRoleTable.Rows.Clear();
            }
            else
            {
                UserAccountRoleTable.Columns.Add("CreatedBy", typeof(Int64));
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
                    row["CreatedBy"] = CreatedBy;

                }
                UserAccountRoleTable.Rows.Add(row);
            }

            return UserAccountRoleTable;
        }
        public DataTable ConvertToDataTable(List<UserAccountOrgDatatable> models, int id)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(UserAccountOrgDatatable).GetProperties();
            if (UserAccountOrgTable.Columns.Contains("UserID"))
            {
                UserAccountOrgTable.Rows.Clear();
            }
            else
            {
                UserAccountOrgTable.Columns.Add("UserID", typeof(Int64));
            }
            
            foreach (var property in properties)
            {
                if (!UserAccountOrgTable.Columns.Contains(property.Name))
                {
                    UserAccountOrgTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
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
        [JsonIgnore]
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
        [JsonIgnore]
        public string? Guid { get; set; }
        public string? PlatformUser { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? Vendor { get; set; }
        public string? DisplayName { get; set; }
        public long LanguageID { get; set; }
        [JsonIgnore]
        public long TimeZoneID { get; set; }
        [EmailAddress]
        public string? emailID { get; set; }
        public string? ContactNo { get; set; }
        public int RoleID { get; set; }
        public long UserPolicy { get; set; }
        public string? PasswordChange { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public string? AccountLocked { get; set; }
        [JsonIgnore]
        public int Tenant { get; set; }
        public string? Active { get; set; }
        public string? TempDeactive { get; set; }
        
        [JsonIgnore]
        public long CreatedBy { get; set; }
        public long ProfileID { get; set; }
        public DateOnly? UserExpiryDate { get; set; }
        public string? ProfileImg {  get; set; }

    }
   public class GetUserAccountModel
    {
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
        
        public string? MasterGuid { get; set; }
        public string? PlatformUser { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? Vendor { get; set; }
        public string? DisplayName { get; set; }
        public long LanguageID { get; set; }
        [JsonIgnore]
        public long TimeZoneID { get; set; }
        [EmailAddress]
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
        
        public string? CreatedBy { get; set; }
        public long ProfileID { get; set; }
        public DateOnly? UserExpiryDate { get; set; }
        [JsonIgnore]
        public DateTime? UserExpiryDateTime { get; set; }
        public string? ProfileImg { get; set; }
    }
    public class UpdateUserAccountModel
    {
        public DataTable UserAccountOrgTable = new DataTable();
        public DataTable UserAccountRoleTable = new DataTable();
        public string? ProfileImg { get; set; }

        public DataTable ConvertToDataTable(List<RoleNameInUserAccount> models, long CreatedBy, string GUid)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(RoleNameInUserAccount).GetProperties();
            if (UserAccountRoleTable.Columns.Contains("UserGUID"))
            {
                UserAccountRoleTable.Rows.Clear();
            }
            else
            {
                UserAccountRoleTable.Columns.Add("UserGUID", typeof(string));

            }
            foreach (var property in properties)
            {
                if (!UserAccountRoleTable.Columns.Contains(property.Name))
                {
                    UserAccountRoleTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
            }
            if (UserAccountRoleTable.Columns.Contains("CreatedBy"))
            {
                UserAccountRoleTable.Rows.Clear();
            }
            else {
                UserAccountRoleTable.Columns.Add("CreatedBy", typeof(Int64));
            }
            
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserAccountRoleTable.NewRow();
                foreach (var property in properties)
                {
                    if (GUid == "")
                    {
                        row["UserGUID"] = "";
                    }
                    else if (GUid != "")
                    {
                        row["UserGUID"] = GUid;
                    }

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                    row["CreatedBy"] = CreatedBy;

                }
                UserAccountRoleTable.Rows.Add(row);
            }

            return UserAccountRoleTable;
        }
        public DataTable ConvertToDataTable(List<UserAccountOrgDatatable> models, string GUid)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(UserAccountOrgDatatable).GetProperties();
            if (UserAccountOrgTable.Columns.Contains("UserGUID"))
            {
                UserAccountOrgTable.Rows.Clear();
            }
            else
            {
                UserAccountOrgTable.Columns.Add("UserGUID", typeof(string));
            }
            
            foreach (var property in properties)
            {
                if (!UserAccountOrgTable.Columns.Contains(property.Name))
                {
                    UserAccountOrgTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserAccountOrgTable.NewRow();
                foreach (var property in properties)
                {
                    if (GUid == "")
                    {
                        row["UserGUID"] = "";
                    }
                    else if (GUid !="")
                    {
                        row["UserGUID"] = GUid;
                    }
                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                UserAccountOrgTable.Rows.Add(row);
            }

            return UserAccountOrgTable;
        }
        [JsonIgnore]
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }

        public string? MasterGuid { get; set; }
        public string? PlatformUser { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? Vendor { get; set; }
        public string? DisplayName { get; set; }
        public long LanguageID { get; set; }
        [JsonIgnore]
        public long TimeZoneID { get; set; }
        [EmailAddress]
        public string? emailID { get; set; }
        public string? ContactNo { get; set; }
        public int RoleID { get; set; }
        public long UserPolicy { get; set; }
        public string? PasswordChange { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public string? AccountLocked { get; set; }
        [JsonIgnore]
        public int Tenant { get; set; }
        public string? Active { get; set; }
        public string? TempDeactive { get; set; }
        
        [JsonIgnore]
        public long? CreatedBy { get; set; }
        public long ProfileID { get; set; }
        public DateOnly? UserExpiryDate { get; set; }
    }

    public class UserAccountOrgDatatable
    {
        public string? OrgName { get; set; }
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
        public UpdateUserAccountModel? UserAccount { get; set; }
        public List<RoleNameInUserAccount?> RoleNameList { get; set; } = new List<RoleNameInUserAccount?>();
        public List<UserAccountOrgDatatable?> OrgDataTable { get; set; } = new List<UserAccountOrgDatatable?>();
    }
    public class UserAccountInsertRequest
    {
        public UserAccountModel? UserAccount { get; set; }
        public List<RoleNameInUserAccount?> RoleNameList { get; set; } = new List<RoleNameInUserAccount?>();
        public List<UserAccountOrgDatatable?> OrgDataTable { get; set; } = new List<UserAccountOrgDatatable?>();
    }

    public class GetRoleName
    {
        public long Value { get; set; }
        
        public string? Text { get; set; }
    }
    public class DeleteRoleName
    {
        public List<DeleteRoleNameInList> DeleteRoleNames { get; set; } = new List<DeleteRoleNameInList>();
        public DataTable UserAccountDeleteRoleTable = new DataTable();
        public DataTable ConvertToDataTable(List<DeleteRoleNameInList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(DeleteRoleNameInList).GetProperties();
            foreach (var property in properties)
            {
                if (!UserAccountDeleteRoleTable.Columns.Contains(property.Name))
                {
                    UserAccountDeleteRoleTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
                
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
        [JsonIgnore]
        public long? CreatedBy { get; set; }
    }
    public class DeleteRoleNameInList
    {
        public string? UserAccountRoleGuid { get; set; }
    }
    public class RoleName
    {
        [JsonIgnore]
        public long? RoleID {  get; set; }
        public string? RoleGUID { get; set; }
        public DateOnly? RoleNameEffectiveDate { get; set; }
        public string? UserGUID { get; set; }
        [JsonIgnore]
        public long? CreatedBy { get; set; }
    }
    public class RoleNameInUserAccount
    {
        public long? RoleID { get; set; }
        public DateOnly? RoleNameEffectiveDate { get; set; }
        
    }

    public class ResetPassword
    {
        [JsonIgnore]
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        [JsonIgnore]
        public long? CreatedBy { get; set; }
        public long? LevelID { get; set; }
        public long? LevelDetailID { get; set; }
        
        [JsonIgnore]
        public string? UserGuid { get; set; }
    }
    public class UserPolicyName
    {
        public string? Value { get; set; }
        public string? Text { get; set; }
    }
    public class UserLanguageName
    {
        public string? Value { get; set; }
        public string? Text { get; set; }
    }
    public class UserTimeZoneName
    {
        public string? Value { get; set; }
        public string? Text { get; set; }
    }
    public class GetUserAccountRole
    {
        public string? UserAccountRoleGuid { get; set; }
        public long? RoleID { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? AccessToAllClient { get; set; }

    }
    public class GetUserAccountOrg
    {
        public long? ID { get; set; }
        public string? OrgCode { get; set; }
        public string? Selected { get; set; }
    }
    public class GetUserAccount
    {
        public long? UserID { get; set; }
        public string? UserName { get; set; }
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
        public string? TimeZone { get; set; }
        public string? LanguageName { get; set; }
        public long? LanguageID { get; set; }
        public string? UserImage { get; set; }
    }
    public class UserAccountResponse
    {
        public GetUserAccount? User { get; set; }
        public List<GetUserAccountRole>? Roles { get; set; }
        public List<GetUserAccountOrg>? Organizations { get; set; }
    }
    public class UnlockUser
    {
        public List<UnlockUserList?> Users
        {
            get; set;
        }
        [JsonIgnore]
        public long? UpdatedBy { get; set; }
        public DataTable UnlockTable = new DataTable();
        public DataTable ConvertToDataTable(List<UnlockUserList?> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(UnlockUserList).GetProperties();
            foreach (var property in properties)
            {
                if (!UnlockTable.Columns.Contains(property.Name))
                {
                    UnlockTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
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
        public string? UserGuId { get; set; }
        
    }
    
    public class DeleteUserAccount
    {
        public DataTable UserAccountDeleteTable = new DataTable();

        public DataTable ConvertToDataTable(List<DeleteUserAccountList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(DeleteUserAccountList).GetProperties();

            foreach (var property in properties)
            {

                UserAccountDeleteTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserAccountDeleteTable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                UserAccountDeleteTable.Rows.Add(row);
            }

            return UserAccountDeleteTable;
        }


        public List<DeleteUserAccountList>? DeleteDataTable { get; set; }

    }
    public class DeleteUserAccountList
    {
        public string? UserGUID { get; set; }
    }
    public class DeleteResult
    {
        public long? SNo { get; set; }
        public string? Result { get; set; }
        public string? Remarks { get; set; }
        public string? UserName { get; set; }
    }
    
}
