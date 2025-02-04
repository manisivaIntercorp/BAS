using DataAccessLayer.Model;

namespace DataAccessLayer.Interface
{
    public interface IUserAccountDAL
    {
        Task<List<UserAccountModel?>> GetAllUserAccount();

        Task<List<UserPolicyName?>> getAllUserPolicyinDropdown();

        Task<List<GetRoleName?>> getAllUserRoleinDropdown();

        Task<(List<UserAccountModel?> insertroles,int? RetVal, string? Msg)> InsertUpdateUserAccount(UserAccountModel? UM);

        Task<(List<DeleteRoleName?> deleteroles, int? RetVal, string? Msg)> DeleteRoleinUserAccount(DeleteRoleName RM);
        
        Task<(List<RoleName?> Roles, int? RetVal, string? Msg)> AddRoleName(RoleName roleName);
        Task<(List<ResetPassword?> PasswordReset, int? RetVal, string? Msg)> ResetPasswordinUserAccount(ResetPassword Passwordreset);
        
        Task<bool> DeleteUserAccount(int? Id);
        
        Task<(GetUserAccount? userAccounts, List<GetUserAccountRole>? UserRoles, List<GetUserAccountOrg>? Org)> GetUserAccountById(int? Id);
        
        Task<List<OrgDetails?>> GetOrgDetailsByUserId();
        Task<(List<UserAccountModel?> updateuseraccount, int? RetVal, string? Msg)> UpdateUserAccountAsync(int? id, UserAccountModel? userAccount);
        
        Task<(List<UnlockUser?> unlockuser, int? RetVal, string? Msg)> UnlockUserAsync(UnlockUser? model);
    }
}
