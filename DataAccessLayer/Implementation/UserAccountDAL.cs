using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class UserAccountDAL: RepositoryBase,IUserAccountDAL
    {
        public UserAccountDAL(IDbTransaction _transaction) : base(_transaction)
        {

        }
        public async Task<bool> DeleteUserAccount(int Id)
        {
            //try {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", Id);
            parameters.Add("@Mode", "DELETE");
            var Result = await Connection.ExecuteAsync("sp_UserAccountCreation",
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

        public async Task<List<UserAccountModel>> GetAllUserAccount()
        {
            //try
            //{
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "GET");
            parameters.Add("@UserId", 0);
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<UserAccountModel>().ToList();
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

        }

        public async Task<UserAccountModel> GetUserAccountById(int Id)
        {
            //try
            //{
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", Id);
            parameters.Add("@Mode", "GET");
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<UserAccountModel>().First();

            return res;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<bool> UpdateUserAccountAsync(int id, UserAccountModel model)
        {
            //try
            //{
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", id);
            parameters.Add("@UserName", model.UserName);
            parameters.Add("@Password", model.UserPassword);
            parameters.Add("@UserPolicy", model.UserPolicy);
            parameters.Add("@Language", model.LanguageID);
            parameters.Add("@Vendor", model.Vendor);
            parameters.Add("@Country", model.CountryID);
            parameters.Add("@ContactNo", model.ContactNo);
            parameters.Add("@TimeZone", model.TimeZoneID);
            parameters.Add("@DisplayName", model.DisplayName);
            parameters.Add("@AccountLocked", model.AccountLocked);
            parameters.Add("@RoleID", model.RoleID);
            parameters.Add("@PasswordChange", model.PasswordChange);
            parameters.Add("@Active", model.Active);
            parameters.Add("@Tenant", model.Tenant);
            parameters.Add("@TempDeactive", model.TempDeactive);
            parameters.Add("@EmailID", model.emailID);
            parameters.Add("@SystemUser", model.SystemUser);
            parameters.Add("@ProfileUser", model.ProfileUser);
            parameters.Add("@PlatformUser", model.PlatformUser);
            parameters.Add("@EffectiveDate", model.EffectiveDate);
            parameters.Add("@PasswordExpiryDate", model.PasswordExpiryDate);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@ProfileID", model.ProfileID);

            parameters.Add("@Mode", "EDIT");
            var multi = await Connection.ExecuteAsync("sp_UserAccountCreation",
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
        public async Task<bool> InsertUpdateUserAccount(UserAccountModel model)
        {
            //try
            //{
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", model.UserId);
            parameters.Add("@UserName", model.UserName);
            parameters.Add("@Password", model.UserPassword);
            parameters.Add("@UserPolicy", model.UserPolicy);
            parameters.Add("@Language", model.LanguageID);
            parameters.Add("@Vendor", model.Vendor);
            parameters.Add("@Country", model.CountryID);
            parameters.Add("@ContactNo", model.ContactNo);
            parameters.Add("@TimeZone", model.TimeZoneID);
            parameters.Add("@DisplayName", model.DisplayName);
            parameters.Add("@AccountLocked", model.AccountLocked);
            parameters.Add("@RoleID", model.RoleID);
            parameters.Add("@PasswordChange",model.PasswordChange);
            parameters.Add("@Active",model.Active);
            parameters.Add("@Tenant", model.Tenant);
            parameters.Add("@TempDeactive",model.TempDeactive);
            parameters.Add("@EmailID", model.emailID);
            parameters.Add("@SystemUser", model.SystemUser);
            parameters.Add("@ProfileUser", model.ProfileUser);
            parameters.Add("@PlatformUser", model.PlatformUser);
            parameters.Add("@EffectiveDate", model.EffectiveDate);
            parameters.Add("@PasswordExpiryDate", model.PasswordExpiryDate);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@ProfileID", model.ProfileID);

            parameters.Add("@Mode", "ADD");
            var multi = await Connection.ExecuteAsync("sp_UserAccountCreation",
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
