namespace DataAccessLayer.Model
{
    public class UserGroupModel
    {
        public long UserGroupID { get; set; }
        public string? UserGroupCode { get; set; }
        public string? RestrictFailedLogin { get; set; }
        public long FailedLoginCount { get; set; }
        public string? PasswordExpiry { get; set; }
        public long PasswordExpiryDays { get; set; }
        public long PasswordExpiryAlertDays { get; set; }
        public string RestrictPasswordReuse { get; set; }
        public string? Active { get; set; }
        public long CreatedBy { get; set; }
        
        public string twoFAAuthentication { get; set; }
        public long LevelID {  get; set; }
        public long LevelDetailsID { get; set; }
        public int IdpBasedUser {  get; set; }
        public long PasswordCount { get; set; }

    }
}
