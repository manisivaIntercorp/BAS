using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Newtonsoft.Json;
using System.Data;

namespace DataAccessLayer.Implementation
{
    public class UserGroupDAL: RepositoryBase, IUserGroupDAL
    {
        public UserGroupDAL(IDbTransaction _transaction) : base(_transaction)
        {
        }
        public async Task<(bool deleteuserGroup, List<DeleteUserGroupResult> deleteResults)> DeleteUserPolicy(long UpdatedBy, DeleteUserGroup deleteUserPolicy)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblDelete", JsonConvert.SerializeObject(deleteUserPolicy.UserGroupDeleteTable));
            parameters.Add("@UpdatedBy", UpdatedBy);
            parameters.Add("@Mode", "DELETE");
            var Result = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            List<DeleteUserGroupResult> DeleteUserAccount = (await Result.ReadAsync<DeleteUserGroupResult>()).ToList();
            while (!Result.IsConsumed)
            {
                await Result.ReadAsync();
            }

            bool res = DeleteUserAccount.Any();



            return (res, DeleteUserAccount.ToList());
            
        }

        public async Task<List<GetUserGroupModel?>> GetAllUserPolicy()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupGUID", string.Empty);
            parameters.Add("@Mode", "GET");
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<GetUserGroupModel?>().ToList();
        }

        public async Task<GetUserGroupModel?> GetUserPolicyByGUId(string? GUId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupGUID", GUId);
            parameters.Add("@Mode", "GET");
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<GetUserGroupModel?>().First();

            return res;
            
        }

        public async Task<(bool InsertUserGroup, int RetVal, string Msg)> InsertUpdateUserPolicy(UserGroupModel? model)
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
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var UserGroup = (await multi.ReadAsync<UserGroupModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }

            bool res = UserGroup.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            return (res, RetVal, Msg);
            
        }

        public async Task<(bool UpdateUserGroup, int RetVal, string Msg)> UpdateUserPolicyAsync(UpdateUserGroupModel? model)
        {
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", model?.UserGroupID);
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
            parameters.Add("@UserGroupGUID", model?.UserPolicyGuid);
            parameters.Add("@Mode", "EDIT");
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var UserGroup = (await multi.ReadAsync<UserGroupModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }

            bool res = UserGroup.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            return (res, RetVal, Msg);
        }
    }
}
