using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Newtonsoft.Json;
using System;
using System.Data;

namespace DataAccessLayer.Implementation
{
    public class UserAccountDAL : RepositoryBase, IUserAccountDAL
    {
        public UserAccountDAL(IDbTransaction _transaction) : base(_transaction)
        {

        }
        public async Task<bool> DeleteUserAccount(int Id)
        {
           DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", Id);
            parameters.Add("@Mode", "DELETE");
            var Result = await Connection.ExecuteAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return Result > 0 ? true : false;
        }

        public async Task<List<UserAccountModel>> GetAllUserAccount()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "GET");
            parameters.Add("@UserId", 0);
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<UserAccountModel>().ToList();
        }

        public async Task<List<UserPolicyName>> getAllUserPolicyinDropdown()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "USER_GROUP");
            parameters.Add("@UserId", 0);
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                            parameters,
                                                            transaction: Transaction,
                                                            commandType: CommandType.StoredProcedure);
            return multi.Read<UserPolicyName>().ToList();
        }

        public async Task<List<GetRoleName>> getAllUserRoleinDropdown()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "USER_ROLE");
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                             parameters,
                                                             transaction: Transaction,
                                                             commandType: CommandType.StoredProcedure);
            return multi.Read<GetRoleName>().ToList();
        }

        public async Task<(GetUserAccount userAccounts, List<GetUserAccountRole> UserRoles, List<GetUserAccountOrg> Org)> GetUserAccountById(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", Id);
            parameters.Add("@Mode", "GET_DETAIL");
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                             parameters,
                                                             transaction: Transaction,
                                                             commandType: CommandType.StoredProcedure);
            var UserAccount =  (await multi.ReadAsync<GetUserAccount>()).FirstOrDefault();
            var RoleDetails = (await multi.ReadAsync<GetUserAccountRole>()).ToList();
            var UserAccountOrg = (await multi.ReadAsync<GetUserAccountOrg>()).ToList();
            return (UserAccount, RoleDetails, UserAccountOrg);
        }

        public async Task<List<OrgDetails>> GetOrgDetailsByUserId()
        {
          DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", 0);
            parameters.Add("@Mode", "GET_ORG");
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<OrgDetails>().ToList();

            return res;
           
        }
        public async Task<(List<UserAccountModel> updateuseraccount, int RetVal, string Msg)> UpdateUserAccountAsync(int id, UserAccountModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", id);
            parameters.Add("@UserName", model.UserName);
            parameters.Add("@Password", model.UserPassword);
            parameters.Add("@UserPolicy", model.UserPolicy);
            parameters.Add("@Language", model.LanguageID);
            parameters.Add("@Vendor", model.Vendor);
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
            parameters.Add("@PasswordExpiryDate", model.PasswordExpiryDate);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@ProfileID", model.ProfileID);
            parameters.Add("@EffectiveDate", model.EffectiveDate);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@Mode", "EDIT");
            using (var result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure))
            {
                // Process the first result set (roles)
                var useraccount = result.Read<UserAccountModel>().ToList();
                // Ensure all result sets are consumed to retrieve the output parameters
                while (!result.IsConsumed)
                {
                    result.Read(); // Process remaining datasets
                }

                // Access the output parameters after consuming all datasets
                int retVal = parameters.Get<int?>("@RetVal") ?? -4;
                string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

                // Return the roles list along with output parameters
                return (useraccount, retVal, msg);
            }
            //var multi = await Connection.ExecuteAsync("sp_UserAccountCreation",
            //                                            parameters,
            //                                            transaction: Transaction,
            //                                            commandType: CommandType.StoredProcedure);
            //var res = multi > 0 ? true : false;

            //return res;
        }
        public async Task<(List<UnlockUser> unlockuser, int RetVal, string Msg)> UnlockUserAsync(UnlockUser model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UpdatedBy", model.UpdatedBy);
            string jsonUserAccount = JsonConvert.SerializeObject(model.UnlockTable);

            parameters.Add("@dtUserAccount", jsonUserAccount, DbType.String);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@Mode", "UNLOCK");
            using (var result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure))
            {
                // Process the first result set (roles)
                var useraccount = result.Read<UnlockUser>().ToList();
                // Ensure all result sets are consumed to retrieve the output parameters
                while (!result.IsConsumed)
                {
                    result.Read(); // Process remaining datasets
                }

                // Access the output parameters after consuming all datasets
                int retVal = parameters.Get<int?>("@RetVal") ?? -4;
                string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

                // Return the roles list along with output parameters
                return (useraccount, retVal, msg);
            }
            
            
        }
        public async Task<(List<UserAccountModel> insertroles, int RetVal, string Msg)> InsertUpdateUserAccount(UserAccountModel model)
        {

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", model.UserId);
            parameters.Add("@UserName", model.UserName);
            parameters.Add("@Password", model.UserPassword);
            parameters.Add("@UserPolicy", model.UserPolicy);
            parameters.Add("@Language", model.LanguageID);
            parameters.Add("@Vendor", model.Vendor);
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
            parameters.Add("@PasswordExpiryDate", model.PasswordExpiryDate);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@ProfileID", model.ProfileID);
            parameters.Add("@EffectiveDate", model.EffectiveDate);
            parameters.Add("@dtOrgRights", JsonConvert.SerializeObject(model.UserAccountOrgTable), DbType.String);
            parameters.Add("@dtOrgRole", JsonConvert.SerializeObject(model.UserAccountRoleTable), DbType.String);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@Mode", "ADD");
            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                using (var result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure))
                {
                    // Process the first result set (roles)
                    var useraccount = result.Read<UserAccountModel>().ToList();

                    // Ensure all result sets are consumed to retrieve the output parameters
                    while (!result.IsConsumed)
                    {
                        result.Read(); // Process remaining datasets
                    }

                    // Access the output parameters after consuming all datasets
                    int retVal = parameters.Get<int?>("@RetVal") ?? -4;
                    string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

                    // Return the roles list along with output parameters
                    return (useraccount, retVal, msg);
                }
                
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("There is already an open DataReader"))
            {
                // Log the exception or handle it
                Console.WriteLine("An open DataReader was detected. Ensure proper disposal.");
                throw;
            }
        }

        public async Task<(List<DeleteRoleName> deleteroles, int RetVal, string Msg)> DeleteRoleinUserAccount(DeleteRoleName model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UpdatedBy",model.CreatedBy);
            parameters.Add("@dtUserAccountRole", JsonConvert.SerializeObject(model.UserAccountDeleteRoleTable), DbType.String);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@Mode", "DELETE_ROLE");
            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                using (var result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure))
                {
                    // Process the first result set (roles)
                    var roles = result.Read<DeleteRoleName>().ToList();

                    // Ensure all result sets are consumed to retrieve the output parameters
                    while (!result.IsConsumed)
                    {
                        result.Read(); // Process remaining datasets
                    }

                    // Access the output parameters after consuming all datasets
                    int retVal = parameters.Get<int?>("@RetVal") ?? -4;
                    string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

                    // Return the roles list along with output parameters
                    return (roles, retVal, msg);
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("There is already an open DataReader"))
            {
                // Log the exception or handle it
                Console.WriteLine("An open DataReader was detected. Ensure proper disposal.");
                throw;
            }
        }

        public async Task<(List<RoleName> Roles, int RetVal, string Msg)> AddRoleName(RoleName model)
        {
            DynamicParameters parameters = new DynamicParameters();
            DateOnly date = (DateOnly)model.RoleNameEffectiveDate;
           parameters.Add("@UserID",model.CreatedBy);
            
            parameters.Add("@EffectiveDate", date.ToString("yyyy-MM-dd"));
            parameters.Add("@RoleID",model.RoleID);
            parameters.Add("@Mode", "ADD_ROLE");
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String,size:200, direction: ParameterDirection.Output);

            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                using (var result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure))
                {
                    // Process the first result set (roles)
                    var roles = result.Read<RoleName>().ToList();

                    // Ensure all result sets are consumed to retrieve the output parameters
                    while (!result.IsConsumed)
                    {
                        result.Read(); // Process remaining datasets
                    }

                    // Access the output parameters after consuming all datasets
                    int retVal = parameters.Get<int?>("@RetVal") ?? -4;
                    string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

                    // Return the roles list along with output parameters
                    return (roles, retVal, msg);  
                }
                
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("There is already an open DataReader"))
            {
                // Log the exception or handle it
                Console.WriteLine("An open DataReader was detected. Ensure proper disposal.");
                throw;
            }

        }

        public async Task<(List<ResetPassword> PasswordReset, int RetVal, string Msg)> ResetPasswordinUserAccount(ResetPassword model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserID", model.UserId);
            parameters.Add("@UserName", model.UserName);
            parameters.Add("@Password",model.Password);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@Mode", "RESET_PWD_MASTER");
            parameters.Add("@TimeZoneID", model.TimeZoneID);
            parameters.Add("@LevelID", model.LevelID);
            parameters.Add("@LevelDetailID", model.LevelDetailID);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                using (var result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure))
                {
                    // Process the first result set (Passwordreset)
                    var Passwordreset = result.Read<ResetPassword>().ToList();

                    // Ensure all result sets are consumed to retrieve the output parameters
                    while (!result.IsConsumed)
                    {
                        result.Read(); // Process remaining datasets
                    }

                    // Access the output parameters after consuming all datasets
                    int retVal = parameters.Get<int?>("@RetVal") ?? -4;
                    string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

                    // Return the Password Reset list along with output parameters
                    return (Passwordreset, retVal, msg);
                }

            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("There is already an open DataReader"))
            {
                // Log the exception or handle it
                Console.WriteLine("An open DataReader was detected. Ensure proper disposal.");
                throw;
            }

        }


    }
}