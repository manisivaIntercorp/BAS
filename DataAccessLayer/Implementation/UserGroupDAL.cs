using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System.Data;

namespace DataAccessLayer.Implementation
{
    public class UserGroupDAL: RepositoryBase, IUserGroupDAL
    {
        public UserGroupDAL(IDbTransaction _transaction) : base(_transaction)
        {
        }
        public async Task<bool> DeleteUserPolicy(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", Id);
            parameters.Add("@Mode", "DELETE");
            var Result = await Connection.ExecuteAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return Result > 0 ? true : false;
        }

        public async Task<List<UserGroupModel?>> GetAllUserPolicy()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", 0);
            parameters.Add("@Mode", "GET");
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<UserGroupModel?>().ToList();
        }

        public async Task<UserGroupModel?> GetUserPolicyById(int? Id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", Id);
            parameters.Add("@Mode", "GET");
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<UserGroupModel?>().First();

            return res;
            
        }

        public async Task<bool> InsertUpdateUserPolicy(UserGroupModel? model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", model?.UserGroupID);
            parameters.Add("@UserGroup", model?.UserGroupCode);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@RestrictFailedLogin", model?.RestrictFailedLogin);
            parameters.Add("@FailedLoginCount",model?.FailedLoginCount);
            parameters.Add("@PasswordExpiry", model?.PasswordExpiry);
            parameters.Add("@PasswordExpiryDays", model?.PasswordExpiryDays);
            parameters.Add("@PasswordExpiryAlertDays", model?.PasswordExpiryAlertDays);
            parameters.Add("@RestrictPasswordReuse", model?.RestrictPasswordReuse);
            parameters.Add("@PasswordCount", model?.PasswordCount);
            parameters.Add("@2FAAuthentication", model?.twoFAAuthentication);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@LevelDetailID", model?.LevelDetailsID);
            parameters.Add("@LevelID",model?.LevelID);
            parameters.Add("@IdpUser", model?.IdpBasedUser);
            
            parameters.Add("@Mode", "ADD");
            var multi = await Connection.ExecuteAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi > 0 ? true : false;

            return res;
            
        }

        public async Task<bool> UpdateUserPolicyAsync(int? id,UserGroupModel? model)
        {
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", id);
            parameters.Add("@UserGroup", model?.UserGroupCode);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@RestrictFailedLogin", model?.RestrictFailedLogin);
            parameters.Add("@FailedLoginCount", model?.FailedLoginCount);
            parameters.Add("@PasswordExpiry", model?.PasswordExpiry);
            parameters.Add("@PasswordExpiryDays", model?.PasswordExpiryDays);
            parameters.Add("@RestrictPasswordReuse", model?.RestrictPasswordReuse);
            parameters.Add("@PasswordCount", model?.PasswordCount);
            parameters.Add("@2FAAuthentication", model?.twoFAAuthentication);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@LevelDetailID", model?.LevelDetailsID);
            parameters.Add("@LevelID", model?.LevelID);
            parameters.Add("@IdpUser", model?.IdpBasedUser);
            parameters.Add("@Mode", "EDIT");
            var multi = await Connection.ExecuteAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi > 0 ? true : false;

            return res;
        }
    }
}
