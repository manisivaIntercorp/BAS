using DataAccessLayer.Model;

namespace DataAccessLayer.Interface
{
    public interface IUserAccountDAL
    {
        Task<List<GetUserAccountModel?>> GetAllUserAccount();

        Task<List<UserPolicyName?>> getAllUserPolicyInDropdown();

        Task<List<GetRoleName?>> getAllUserRoleInDropdown();

        Task<(List<UserAccountModel?> InsertedUsers, List<OrgDetails?> OrgDetails, int? RetVal, string? Msg)> InsertUpdateUserAccount(UserAccountModel? UM);

        Task<(List<DeleteRoleName?> deleteroles, int? RetVal, string? Msg)> DeleteRoleInUserAccount(DeleteRoleName RM);

        Task<(List<RoleName?> Roles, int? RetVal, string? Msg)> AddRoleName(RoleName roleName);
        Task<(List<ResetPassword?> PasswordReset, int? RetVal, string? Msg)> ResetPasswordInUserAccount(ResetPassword PasswordReset);

        Task<(bool deleteuseraccount, List<DeleteResult> deleteResults)> DeleteUserAccount(long UpdatedBy,DeleteUserAccount deleteUserAccount);

        Task<(GetUserAccount? userAccounts, List<GetUserAccountRole>? UserRoles, List<GetUserAccountOrg>? Org)> GetUserAccountByGUId(string? GUId);
        Task<List<OrgDetails?>> GetOrgDetailsByUserGUId();

        Task<(List<UserAccountModel?> updateuseraccount, int? RetVal, string? Msg)> UpdateUserAccountAsync(UserAccountModel? userAccount);


        Task<(List<UserAccountModel?> updateuseraccount, int? RetVal, string? Msg)> EditUpdateUserAccountAsync(UserAccountModel? userAccount);

        Task<(List<UnlockUser?> unlockuser, int? RetVal, string? Msg)> UnlockUserAsync(UnlockUser? model);
    }
}
