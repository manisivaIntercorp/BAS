using System.Data;
using System.Text.Json.Serialization;

namespace DataAccessLayer.Model
{
    public class UserGroupModel
    {
        [JsonIgnore]
        public long UserGroupID { get; set; }
        public string? UserGroupCode { get; set; }
        public string? RestrictFailedLogin { get; set; }
        public long FailedLoginCount { get; set; }
        public string? PasswordExpiry { get; set; }
        public long PasswordExpiryDays { get; set; }
        public long PasswordExpiryAlertDays { get; set; }
        public string? RestrictPasswordReuse { get; set; }
        public string? Active { get; set; }
        [JsonIgnore]
        public long CreatedBy { get; set; }
        
        public string? twoFAAuthentication { get; set; }
        public long LevelID {  get; set; }
        public long LevelDetailsID { get; set; }
        public int IdpBasedUser {  get; set; }
        public long PasswordCount { get; set; }
        
    }

    public class GetUserGroupModel
    {
        public long UserGroupID { get; set; }
        public string? UserGroupCode { get; set; }
        public string? RestrictFailedLogin { get; set; }
        public long FailedLoginCount { get; set; }
        public string? PasswordExpiry { get; set; }
        public long PasswordExpiryDays { get; set; }
        public long PasswordExpiryAlertDays { get; set; }
        public string? RestrictPasswordReuse { get; set; }
        public string? Active { get; set; }
        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public string? twoFAAuthentication { get; set; }
        public long LevelID { get; set; }
        public long LevelDetailsID { get; set; }
        public int IdpBasedUser { get; set; }
        public long PasswordCount { get; set; }
        public string? UserPolicyGuid { get; set; }
    }
    public class UpdateUserGroupModel
    {
        [JsonIgnore]
        public long UserGroupID { get; set; }
        public string? UserGroupCode { get; set; }
        public string? RestrictFailedLogin { get; set; }
        public long FailedLoginCount { get; set; }
        public string? PasswordExpiry { get; set; }
        public long PasswordExpiryDays { get; set; }
        public long PasswordExpiryAlertDays { get; set; }
        public string? RestrictPasswordReuse { get; set; }
        public string? Active { get; set; }
        [JsonIgnore]
        public long CreatedBy { get; set; }

        public string? twoFAAuthentication { get; set; }
        public long LevelID { get; set; }
        public long LevelDetailsID { get; set; }
        public int IdpBasedUser { get; set; }
        public long PasswordCount { get; set; }
        public string? UserPolicyGuid { get; set; }
    }

    public class DeleteUserGroup
    {
        public DataTable UserGroupDeleteTable = new DataTable();

        public DataTable ConvertToDataTable(List<DeleteUserGroupList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(DeleteUserGroupList).GetProperties();

            foreach (var property in properties)
            {

                UserGroupDeleteTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserGroupDeleteTable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                UserGroupDeleteTable.Rows.Add(row);
            }

            return UserGroupDeleteTable;
        }


        public List<DeleteUserGroupList>? DeleteDataTable { get; set; }

    }
    public class DeleteUserGroupList
    {
        public string? UserPolicyGUID { get; set; }

    }
    public class DeleteUserGroupResult
    {
        public long? SNo { get; set; }
        public string? Result { get; set; }
        public string? Remarks { get; set; }
        public string? UserGroupName { get; set; }
    }
}
