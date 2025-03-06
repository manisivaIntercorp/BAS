using DataAccessLayer.Model;

namespace DataAccessLayer.Interface
{
    public interface IUserAccountDAL
    {
        Task<List<GetUserAccountModel?>> GetAllUserAccount(long UpdatedBy);

        Task<List<UserPolicyName?>> getAllUserPolicyInDropdown();

        Task<List<UserLanguageName?>> getAllUserLanguageInDropdown();
        Task<List<UserTimeZoneName?>> getAllUserTimeZoneInDropdown();

        Task<List<GetRoleName?>> getAllUserRoleInDropdown();
        Task<(List<UserAccountModel?> InsertedUsers,  long? RetVal, string? Msg)> InsertCheckUserAccount(UserAccountModel? UM);
        Task<(List<UserAccountModel?> InsertedUsers, List<OrgDetails?> OrgDetails, long? RetVal, string? Msg)> InsertUpdateUserAccount(UserAccountModel? UM);

        Task<(List<DeleteRoleName?> deleteroles, int? RetVal, string? Msg)> DeleteRoleInUserAccount(DeleteRoleName RM);

        Task<(List<RoleName?> Roles, int? RetVal, string? Msg)> AddRoleName(RoleName roleName);
        Task<(List<ResetPassword?> PasswordReset, int? RetVal, string? Msg)> ResetPasswordInUserAccount(ResetPassword PasswordReset);

        Task<(bool deleteuseraccount, List<DeleteResult> deleteResults)> DeleteUserAccount(long UpdatedBy,DeleteUserAccount deleteUserAccount);

        Task<(GetUserAccount? userAccounts, List<GetUserAccountRole>? UserRoles, List<GetUserAccountOrg>? Org)> GetUserAccountByGUId(string? GUId);
        Task<List<OrgDetails?>> GetOrgDetailsByUserGUId();

        Task<(List<UpdateUserAccountModel?> updateuseraccount, List<OrgDetails?> OrgDetails, long? RetVal, string? Msg)> UpdateUserAccountAsync(UpdateUserAccountModel? userAccount);


        Task<(List<UpdateUserAccountModel?> updateuseraccount, int? RetVal, string? Msg)> EditUpdateUserAccountAsync(UpdateUserAccountModel? userAccount);

        Task<(List<UnlockUser?> unlockuser, int? RetVal, string? Msg)> UnlockUserAsync(UnlockUser? model);
    }
}
