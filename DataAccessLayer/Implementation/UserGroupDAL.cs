using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class UserGroupDAL: RepositoryBase, IUserGroupDAL
    {
        public UserGroupDAL(IDbTransaction _transaction) : base(_transaction)
        {
        }
        public async Task<bool> DeleteUserPolicy(int Id)
        {
            //try {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", Id);
            parameters.Add("@Mode", "DELETE");
            var Result = await Connection.ExecuteAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return Result > 0 ? true : false;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

        }

        public async Task<List<UserGroupModel>> GetAllUserPolicy()
        {
            //try
            //{
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", 0);
            parameters.Add("@Mode", "GET");
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<UserGroupModel>().ToList();
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

        }

        public async Task<UserGroupModel> GetUserPolicyById(int Id)
        {
            //try
            //{
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", Id);
            parameters.Add("@Mode", "GET");
            var multi = await Connection.QueryMultipleAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<UserGroupModel>().First();

            return res;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<bool> InsertUpdateUserPolicy(UserGroupModel model)
        {
            //try
            //{
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", model.UserGroupID);
            parameters.Add("@UserGroup", model.UserGroupCode);
            parameters.Add("@Active", model.Active);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@RestrictFailedLogin", model.RestrictFailedLogin);
            parameters.Add("@FailedLoginCount",model.FailedLoginCount);
            parameters.Add("@PasswordExpiry", model.PasswordExpiry);
            parameters.Add("@PasswordExpiryDays", model.PasswordExpiryDays);
            parameters.Add("@PasswordExpiryAlertDays", model.PasswordExpiryAlertDays);
            parameters.Add("@RestrictPasswordReuse", model.RestrictPasswordReuse);
            parameters.Add("@PasswordCount", model.PasswordCount);
            parameters.Add("@2FAAuthentication", model.twoFAAuthentication);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@LevelDetailID", model.LevelDetailsID);
            parameters.Add("@LevelID",model.LevelID);
            parameters.Add("@IdpUser", model.IdpBasedUser);
            
            parameters.Add("@Mode", "ADD");
            var multi = await Connection.ExecuteAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi > 0 ? true : false;

            return res;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<bool> UpdateUserPolicyAsync(int id,UserGroupModel model)
        {
            //try
            //{
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGroupID", id);
            parameters.Add("@UserGroup", model.UserGroupCode);
            parameters.Add("@Active", model.Active);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@RestrictFailedLogin", model.RestrictFailedLogin);
            parameters.Add("@FailedLoginCount", model.FailedLoginCount);
            parameters.Add("@PasswordExpiry", model.PasswordExpiry);
            parameters.Add("@PasswordExpiryDays", model.PasswordExpiryDays);
            parameters.Add("@RestrictPasswordReuse", model.RestrictPasswordReuse);
            parameters.Add("@PasswordCount", model.PasswordCount);
            parameters.Add("@2FAAuthentication", model.twoFAAuthentication);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@LevelDetailID", model.LevelDetailsID);
            parameters.Add("@LevelID", model.LevelID);
            parameters.Add("@IdpUser", model.IdpBasedUser);
            parameters.Add("@Mode", "EDIT");
            var multi = await Connection.ExecuteAsync("sp_UserGroup",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi > 0 ? true : false;

            return res;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

        }
    }
}
