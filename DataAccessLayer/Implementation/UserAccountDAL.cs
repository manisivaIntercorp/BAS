using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using DataAccessLayer.Services;
using Newtonsoft.Json;
using Microsoft.SqlServer.Management.Smo;
using System.Net.Http;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.SqlServer.Management.XEvent;
using System.Reflection;

namespace DataAccessLayer.Implementation
{
    public class UserAccountDAL : RepositoryBase, IUserAccountDAL
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly EncryptedDecrypt? _encryptedDecrypt;
        public UserAccountDAL(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(new SqlConnection(configuration.GetConnectionString("connection"))) // ✅ Always Start with Master DB
        {
            _httpContextAccessor = httpContextAccessor?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _encryptedDecrypt = new EncryptedDecrypt(configuration);
            SetDynamicConnection();
        }

        // 🔹 Fetch and Set DB Connection from Middleware (HttpContext.Items)
        private void SetDynamicConnection()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null && httpContext.Items.ContainsKey("connection"))
            {
                string dynamicConnectionString = httpContext.Items["connection"] as string ?? string.Empty;

                if (!string.IsNullOrEmpty(dynamicConnectionString))
                {
                    Connection = new SqlConnection(dynamicConnectionString);
                }
            }
        }

        // 🔹 Method to Change Connection Dynamically (if Needed)
        private void UpdateConnectionString(string? dbName, string? server, string? user, string? password)
        {
            
            string? masterConnection = _configuration.GetConnectionString("connection");

            var builder = new SqlConnectionStringBuilder(masterConnection)
            {
                DataSource = _encryptedDecrypt?.Decrypt(server),
                UserID = _encryptedDecrypt?.Decrypt(user),
                Password = _encryptedDecrypt?.Decrypt(password),
                InitialCatalog = dbName
            };

            // 🌟 Update DAL Connection
            Connection = new SqlConnection(builder.ToString());

            // 🌟 Store Updated Connection in HttpContext.Items (For Middleware)
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Items["connection"] = builder.ToString();
                httpContext.Session.SetString("DBName", dbName??string.Empty);
                httpContext.Session.SetString("InstanceName", server??string.Empty);
                httpContext.Session.SetString("DataBaseUserName", user??string.Empty);
                httpContext.Session.SetString("DataBasePassword", password??string.Empty);
            }
        }

        // 🔹 Fetch Org Details and Update Connection String
        public async Task<List<OrgDetails?>> GetOrgDetailsByUserId(int? userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@Mode", "GET_ORG");

            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);

            var orgDetails = multi.Read<OrgDetails?>().ToList();
            return orgDetails;
        }

        public async Task<List<OrgDetails?>> GetOrgDetailsByUserName(string? username)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName", username);
            parameters.Add("@Mode", "GET_ORG");

            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);

            var orgDetails = multi.Read<OrgDetails?>().ToList();
            return orgDetails;
        }

        // Insert or Update User, Then Change DB Connection
        public async Task<(List<UserAccountModel?> InsertedUsers, List<OrgDetails?> OrgDetails, int? RetVal, string? Msg)> InsertUpdateUserAccount(UserAccountModel? model)
        {
            // To Check the DBName is in Default DB Name is Master DB
            if (_httpContextAccessor.HttpContext != null)
            {
                if(Connection.Database!="MasterData") // To Change the DB Name into Master DB
                {
                    string? masterConnection = _configuration.GetConnectionString("connection");
                    Connection = new SqlConnection(masterConnection);
                }
                else if(model?.Tenant==0)
                {
                    string? masterConnection = _configuration.GetConnectionString("connection");
                    Connection = new SqlConnection(masterConnection);
                }
                if(model?.UserAccountOrgTable!=null)
                {
                    foreach(DataRow row in model?.UserAccountOrgTable.Rows)
                    {
                        var orgname = row["OrgName"] != DBNull.Value ? row["OrgName"]?.ToString() : null;
                        if (string.IsNullOrEmpty(orgname) || row["OrgName"]?.ToString() == "string")
                        {
                            string? masterConnection = _configuration.GetConnectionString("connection");
                            Connection = new SqlConnection(masterConnection);
                        }
                    }
                }
            }
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", model?.UserId);
            parameters.Add("@UserName", model?.UserName);
            parameters.Add("@Password", EncryptShaAlg.Encrypt(model?.UserPassword));
            parameters.Add("@UserPolicy", model?.UserPolicy);
            parameters.Add("@Language", model?.LanguageID);
            parameters.Add("@Vendor", model?.Vendor);
            parameters.Add("@ContactNo", model?.ContactNo);
            parameters.Add("@TimeZone", model?.TimeZoneID);
            parameters.Add("@DisplayName", model?.DisplayName);
            parameters.Add("@AccountLocked", model?.AccountLocked);
            parameters.Add("@RoleID", model?.RoleID);
            parameters.Add("@PasswordChange", model?.PasswordChange);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@Tenant", model?.Tenant);
            parameters.Add("@TempDeactive", model?.TempDeactive);
            parameters.Add("@EmailID", model?.emailID);
            parameters.Add("@SystemUser", model?.SystemUser);
            parameters.Add("@ProfileUser", model?.ProfileUser);
            parameters.Add("@PlatformUser", model?.PlatformUser);
            parameters.Add("@PasswordExpiryDate", model?.PasswordExpiryDate);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@ProfileID", model?.ProfileID);
            parameters.Add("@EffectiveDate", model?.EffectiveDate);
            parameters.Add("@dtOrgRights", JsonConvert.SerializeObject(model?.UserAccountOrgTable), DbType.String);
            parameters.Add("@dtOrgRole", JsonConvert.SerializeObject(model?.UserAccountRoleTable), DbType.String);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@Mode", "ADD");

            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }

                using var result = await Connection.QueryMultipleAsync(
                    "sp_UserAccountCreation",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);

                var InsertedUsers = result.Read<UserAccountModel?>().ToList();
                while (!result.IsConsumed)
                {
                    result.Read();
                }
               
                int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
                model.UserId = (long?)parameters.Get<int?>("@RetVal") ?? -4;
                string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
                List<OrgDetails?> OrgDetails = new List<OrgDetails?>();
                if (RetVal >= 1)
                {
                    if (model?.Tenant != 0)
                    {
                        // 🌟 Fetch Org Details and Change Connection
                        OrgDetails = await GetOrgDetailsByUserId(Convert.ToInt32(RetVal));
                        if (OrgDetails != null && OrgDetails.Any())
                        {
                            var strdbname = _httpContextAccessor?.HttpContext?.Session.GetString("DBName");
                            var strinstancename = _httpContextAccessor?.HttpContext?.Session.GetString("InstanceName");
                            var strusername = _httpContextAccessor?.HttpContext?.Session.GetString("DataBaseUserName");
                            var strpassword = _httpContextAccessor?.HttpContext?.Session.GetString("DataBasePassword");

                            foreach (var org in OrgDetails)
                            {

                                if (org != null)
                                {
                                    UpdateConnectionString(org.DBName, org.InstanceName, org.ConUserName, org.ConPassword);
                                    var ClientResult = await InsertUpdateUserAccountClient(model);

                                }

                            }
                            _httpContextAccessor?.HttpContext?.Session.SetString("DBName", strdbname ?? string.Empty);
                            _httpContextAccessor?.HttpContext?.Session.SetString("InstanceName", strinstancename ?? string.Empty);
                            _httpContextAccessor?.HttpContext?.Session.SetString("DataBaseUserName", strusername ?? string.Empty);
                            _httpContextAccessor?.HttpContext?.Session.SetString("DataBasePassword", strpassword ?? string.Empty);

                        }
                    }

                }
                
                return (InsertedUsers??new List<UserAccountModel?>(), OrgDetails ?? new List<OrgDetails?>(), RetVal, Msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
        // To Insert into Client Database
        public async Task<(List<UserAccountModel?> InsertedClientUsers,List<OrgDetails?> OrgClientDetails,int? RetClientVal, string? ClientMsg)> InsertUpdateUserAccountClient(UserAccountModel? model)
        {
            DynamicParameters clientParameters = new DynamicParameters();
            clientParameters.Add("@UserId", model?.UserId);
            clientParameters.Add("@UserName", model?.UserName);
            clientParameters.Add("@Password", EncryptShaAlg.Encrypt(model?.UserPassword));
            clientParameters.Add("@UserPolicy", model?.UserPolicy);
            clientParameters.Add("@Language", model?.LanguageID);
            clientParameters.Add("@Vendor", model?.Vendor);
            clientParameters.Add("@ContactNo", model?.ContactNo);
            clientParameters.Add("@TimeZone", model?.TimeZoneID);
            clientParameters.Add("@DisplayName", model?.DisplayName);
            clientParameters.Add("@AccountLocked", model?.AccountLocked);
            clientParameters.Add("@RoleID", model?.RoleID);
            clientParameters.Add("@PasswordChange", model?.PasswordChange);
            clientParameters.Add("@Active", model?.Active);
            clientParameters.Add("@Tenant", model?.Tenant);
            clientParameters.Add("@TempDeactive", model?.TempDeactive);
            clientParameters.Add("@EmailID", model?.emailID);
            clientParameters.Add("@SystemUser", model?.SystemUser);
            clientParameters.Add("@ProfileUser", model?.ProfileUser);
            clientParameters.Add("@PlatformUser", model?.PlatformUser);
            clientParameters.Add("@PasswordExpiryDate", model?.PasswordExpiryDate);
            clientParameters.Add("@UpdatedBy", model?.CreatedBy);
            clientParameters.Add("@ProfileID", model?.ProfileID);
            clientParameters.Add("@EffectiveDate", model?.EffectiveDate);
            clientParameters.Add("@dtOrgRights", JsonConvert.SerializeObject(model?.UserAccountOrgTable), DbType.String);
            clientParameters.Add("@dtOrgRole", JsonConvert.SerializeObject(model?.UserAccountRoleTable), DbType.String);
            clientParameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            clientParameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            clientParameters.Add("@Mode", "ADD");
            using var resultClient = await Connection.QueryMultipleAsync(
                                                                        "sp_UserAccountCreation",
                                                                        clientParameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure);

            var InsertedUsersClient = resultClient.Read<UserAccountModel?>().ToList();
            while (!resultClient.IsConsumed)
            {
                resultClient.Read();
            }
            int RetValClient = clientParameters.Get<int?>("@RetVal") ?? -4;
            string MsgClient = clientParameters.Get<string?>("@Msg") ?? "No Records Found";
            List<OrgDetails?> OrgDetails = new List<OrgDetails?>();
            
            return (InsertedUsersClient, OrgDetails,RetValClient, MsgClient);
        }

        public async Task<(bool deleteuseraccount, List<DeleteResult> deleteResults)> DeleteUserAccount(int UpdatedBy, DeleteUserAccount deleteUserAccount)
        {
            List<OrgDetails?> OrgDetails = new List<OrgDetails?>();
            if (deleteUserAccount.DeleteDatatable != null)
            {
                foreach (var DeleteResult in deleteUserAccount.DeleteDatatable)
                {
                    // 🌟 Fetch Org Details and Change Connection
                    OrgDetails = await GetOrgDetailsByUserName(DeleteResult.UserName);
                    if (OrgDetails != null && OrgDetails.Any())
                    {
                        var strdbname = _httpContextAccessor?.HttpContext?.Session.GetString("DBName");
                        var strinstancename = _httpContextAccessor?.HttpContext?.Session.GetString("InstanceName");
                        var strusername = _httpContextAccessor?.HttpContext?.Session.GetString("DataBaseUserName");
                        var strpassword = _httpContextAccessor?.HttpContext?.Session.GetString("DataBasePassword");

                        foreach (var org in OrgDetails)
                        {

                            if (org != null)
                            {
                                UpdateConnectionString(org.DBName, org.InstanceName, org.ConUserName, org.ConPassword);
                                var ClientResult = await DeleteClientUserAccount(UpdatedBy, deleteUserAccount);

                            }

                        }
                        _httpContextAccessor?.HttpContext?.Session.SetString("DBName", strdbname ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("InstanceName", strinstancename ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("DataBaseUserName", strusername ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("DataBasePassword", strpassword ?? string.Empty);

                    }

                }
            }
                if (_httpContextAccessor?.HttpContext != null)
                {
                    if (Connection.Database != "MasterData")// To Change the DB Name into Master DB
                    {
                        string? masterConnection = _configuration.GetConnectionString("connection");
                        Connection = new SqlConnection(masterConnection);
                    }
                }
            
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@tblDelete", JsonConvert.SerializeObject(deleteUserAccount.UserAccountDeleteTable));
                parameters.Add("@UpdatedBy", UpdatedBy);
                parameters.Add("@Mode", "DELETE");

                var Result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
                List<DeleteResult> DeleteUserAccount = (await Result.ReadAsync<DeleteResult>()).ToList();
                while (!Result.IsConsumed)
                {
                    await Result.ReadAsync();
                }

                bool res = DeleteUserAccount.Any();



                return (res, DeleteUserAccount.ToList());
            }
        

        // To Delete Client Database
        public async Task<(bool deleteuseraccount, List<DeleteResult>)> DeleteClientUserAccount(int UpdatedBy, DeleteUserAccount deleteUserAccount)
        {

            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblDelete", JsonConvert.SerializeObject(deleteUserAccount.UserAccountDeleteTable));
            parameters.Add("@UpdatedBy", UpdatedBy);
            parameters.Add("@Mode", "DELETE");

            var Result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var deleteuseraccount = (await Result.ReadAsync<DeleteResult>()).ToList();
            while (!Result.IsConsumed)
            {
                await Result.ReadAsync();
            }

            bool res = deleteuseraccount.Any();
           

            return (res, deleteuseraccount.ToList());
        }

        public async Task<List<UserAccountModel?>> GetAllUserAccount()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "GET");
            parameters.Add("@UserId", 0);
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<UserAccountModel?>().ToList();
        }

        public async Task<List<UserPolicyName?>> getAllUserPolicyinDropdown()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "USER_GROUP");
            parameters.Add("@UserId", 0);
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                            parameters,
                                                            transaction: Transaction,
                                                            commandType: CommandType.StoredProcedure);
            return multi.Read<UserPolicyName?>().ToList();
        }

        public async Task<List<GetRoleName?>> getAllUserRoleinDropdown()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "USER_ROLE");
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                             parameters,
                                                             transaction: Transaction,
                                                             commandType: CommandType.StoredProcedure);
            return multi.Read<GetRoleName?>().ToList();
        }

        public async Task<(GetUserAccount? userAccounts, List<GetUserAccountRole>? UserRoles, List<GetUserAccountOrg>? Org)> GetUserAccountById(int? Id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", Id);
            parameters.Add("@Mode", "GET_DETAIL");
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                             parameters,
                                                             transaction: Transaction,
                                                             commandType: CommandType.StoredProcedure);
            var UserAccount = (await multi.ReadAsync<GetUserAccount>())?.FirstOrDefault();
            var RoleDetails = (await multi.ReadAsync<GetUserAccountRole>())?.ToList();
            var UserAccountOrg = (await multi.ReadAsync<GetUserAccountOrg>())?.ToList();
            return (UserAccount, RoleDetails, UserAccountOrg);
        }

        //public async Task<(GetUserAccount? userAccounts, List<GetUserAccountRole>? UserRoles, List<GetUserAccountOrg>? Org)> GetUserAccountByName(string? UserName)
        //{
        //    DynamicParameters parameters = new DynamicParameters();
        //    parameters.Add("@UserName", UserName);
        //    parameters.Add("@Mode", "GET_DETAIL");
        //    var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
        //                                                     parameters,
        //                                                     transaction: Transaction,
        //                                                     commandType: CommandType.StoredProcedure);
        //    var UserAccount = (await multi.ReadAsync<GetUserAccount>())?.FirstOrDefault();
        //    var RoleDetails = (await multi.ReadAsync<GetUserAccountRole>())?.ToList();
        //    var UserAccountOrg = (await multi.ReadAsync<GetUserAccountOrg>())?.ToList();
        //    return (UserAccount, RoleDetails, UserAccountOrg);
        //}

        public async Task<List<OrgDetails?>> GetOrgDetailsByUserId()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", 0);
            parameters.Add("@Mode", "GET_ORG");
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<OrgDetails?>().ToList();

            return res;

        }


        public async Task<(List<UserAccountModel> updateuseraccount, int RetVal, string Msg)> UpdateUserAccountAsync(UserAccountModel? model)
        {
            // To Check the DBName is in Default DB Name is Master DB
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                if (Connection.Database!= "MasterData")// To Change the DB Name into Master DB
                {
                    string? masterConnection = _configuration.GetConnectionString("connection");
                    Connection = new SqlConnection(masterConnection);
                }
                else if (model?.Tenant == 0)
                {
                    string? masterConnection = _configuration.GetConnectionString("connection");
                    Connection = new SqlConnection(masterConnection);
                }
            }
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", 0);
            parameters.Add("@UserName", model?.UserName);
            parameters.Add("@Password", EncryptShaAlg.Encrypt(model?.UserPassword));
            parameters.Add("@UserPolicy", model?.UserPolicy);
            parameters.Add("@Language", model?.LanguageID);
            parameters.Add("@Vendor", model?.Vendor);
            parameters.Add("@ContactNo", model?.ContactNo);
            parameters.Add("@TimeZone", model?.TimeZoneID);
            parameters.Add("@DisplayName", model?.DisplayName);
            parameters.Add("@AccountLocked", model?.AccountLocked);
            parameters.Add("@RoleID", model?.RoleID);
            parameters.Add("@PasswordChange", model?.PasswordChange);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@Tenant", model?.Tenant);
            parameters.Add("@TempDeactive", model?.TempDeactive);
            parameters.Add("@EmailID", model?.emailID);
            parameters.Add("@SystemUser", model?.SystemUser);
            parameters.Add("@ProfileUser", model?.ProfileUser);
            parameters.Add("@PlatformUser", model?.PlatformUser);
            parameters.Add("@PasswordExpiryDate", model?.PasswordExpiryDate);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@ProfileID", model?.ProfileID);
            parameters.Add("@EffectiveDate", model?.EffectiveDate);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            
            parameters.Add("@Mode", "EDIT");
            using (var result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure))
            {
                // Process the first result set (roles)
                var retVal = parameters.Get<int>("@RetVal");
                var message = parameters.Get<string>("@Msg");


                List<UserAccountModel> UserAccount = new List<UserAccountModel>();
                

                List<OrgDetails?> OrgDetails = new List<OrgDetails?>();
                if (retVal == -1 || retVal==null || retVal==0)
                {
                    result.Read<UserAccountModel?>().ToList();
                    return (UserAccount, retVal, message);
                }

                else if (retVal > 0)
                {
                    if (model?.Tenant != 0)
                    {
                        // 🌟 Fetch Org Details and Change Connection
                        OrgDetails = await GetOrgDetailsByUserName(model?.UserName);
                        if (OrgDetails != null && OrgDetails.Any())
                        {
                            var strdbname = _httpContextAccessor?.HttpContext?.Session.GetString("DBName");
                            var strinstancename = _httpContextAccessor?.HttpContext?.Session.GetString("InstanceName");
                            var strusername = _httpContextAccessor?.HttpContext?.Session.GetString("DataBaseUserName");
                            var strpassword = _httpContextAccessor?.HttpContext?.Session.GetString("DataBasePassword");

                            foreach (var org in OrgDetails)
                            {

                                if (org != null)
                                {
                                    UpdateConnectionString(org.DBName, org.InstanceName, org.ConUserName, org.ConPassword);
                                    var ClientResult = await UpdateClientUserAccountAsync(model?.UserName, model);
                                }

                            }
                            _httpContextAccessor?.HttpContext?.Session.SetString("DBName", strdbname ?? string.Empty);
                            _httpContextAccessor?.HttpContext?.Session.SetString("InstanceName", strinstancename ?? string.Empty);
                            _httpContextAccessor?.HttpContext?.Session.SetString("DataBaseUserName", strusername ?? string.Empty);
                            _httpContextAccessor?.HttpContext?.Session.SetString("DataBasePassword", strpassword ?? string.Empty);

                        }
                    }// To Change the Client Connection
                }
                
                // Ensure all result sets are consumed to retrieve the output parameters
                while (!result.IsConsumed)
                {
                    result.Read(); // Process remaining datasets
                }
                return (UserAccount, retVal, message);
            }

        }
        //To Edit the Data in Client DB
        public async Task<(List<UserAccountModel?> updateClientuseraccount, int? RetValClient, string? MsgClient)> UpdateClientUserAccountAsync(string? username, UserAccountModel? model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", model?.UserId);
            parameters.Add("@UserName", username);
            parameters.Add("@Password", model?.UserPassword);
            parameters.Add("@UserPolicy", model?.UserPolicy);
            parameters.Add("@Language", model?.LanguageID);
            parameters.Add("@Vendor", model?.Vendor);
            parameters.Add("@ContactNo", model?.ContactNo);
            parameters.Add("@TimeZone", model?.TimeZoneID);
            parameters.Add("@DisplayName", model?.DisplayName);
            parameters.Add("@AccountLocked", model?.AccountLocked);
            parameters.Add("@RoleID", model?.RoleID);
            parameters.Add("@PasswordChange", model?.PasswordChange);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@Tenant", model?.Tenant);
            parameters.Add("@TempDeactive", model?.TempDeactive);
            parameters.Add("@EmailID", model?.emailID);
            parameters.Add("@SystemUser", model?.SystemUser);
            parameters.Add("@ProfileUser", model?.ProfileUser);
            parameters.Add("@PlatformUser", model?.PlatformUser);
            parameters.Add("@PasswordExpiryDate", model?.PasswordExpiryDate);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@ProfileID", model?.ProfileID);
            parameters.Add("@EffectiveDate", model?.EffectiveDate);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@Mode", "EDIT");
            using (var result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure))
            {
                // Process the first result set (roles)
                var UserAccount = result.Read<UserAccountModel?>().ToList();
                // Ensure all result sets are consumed to retrieve the output parameters
                while (!result.IsConsumed)
                {
                    result.Read(); // Process remaining datasets
                }

                // Access the output parameters after consuming all datasets
                int retVal = parameters.Get<int?>("@RetVal") ?? -4;
                string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
                // Return the roles list along with output parameters
                return (UserAccount, retVal, msg);
            }

        }
        public async Task<(List<UnlockUser?> unlockuser, int? RetVal, string? Msg)> UnlockUserAsync(UnlockUser? model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UpdatedBy", model?.UpdatedBy, dbType: DbType.Int64, direction: ParameterDirection.Input);
            string jsonUserAccount = JsonConvert.SerializeObject(model?.UnlockTable);

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
                var useraccount = result.Read<UnlockUser?>().ToList();
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

        public async Task<(List<DeleteRoleName?> deleteroles, int? RetVal, string? Msg)> DeleteRoleinUserAccount(DeleteRoleName model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UpdatedBy", model.CreatedBy);
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
                    var roles = result.Read<DeleteRoleName?>().ToList();

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

        public async Task<(List<RoleName?> Roles, int? RetVal, string? Msg)> AddRoleName(RoleName model)
        {
            DynamicParameters parameters = new DynamicParameters();
            DateOnly? date = (DateOnly?)model.RoleNameEffectiveDate;
            parameters.Add("@UserID", model.CreatedBy);

            parameters.Add("@EffectiveDate", date?.ToString("yyyy-MM-dd"));
            parameters.Add("@RoleID", model.RoleID);
            parameters.Add("@Mode", "ADD_ROLE");
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
                    // Process the first result set (roles)
                    var roles = result.Read<RoleName?>().ToList();

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

        public async Task<(List<ResetPassword?> PasswordReset, int? RetVal, string? Msg)> ResetPasswordinUserAccount(ResetPassword model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserID", model.UserId);
            parameters.Add("@UserName", model.UserName);
            parameters.Add("@Password", EncryptShaAlg.Encrypt(model.Password));
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
                    var PasswordReset = result.Read<ResetPassword?>().ToList();

                    // Ensure all result sets are consumed to retrieve the output parameters
                    while (!result.IsConsumed)
                    {
                        result.Read(); // Process remaining datasets
                    }

                    // Access the output parameters after consuming all datasets
                    int retVal = parameters.Get<int?>("@RetVal") ?? -4;
                    string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

                    // Return the Password Reset list along with output parameters
                    return (PasswordReset, retVal, msg);
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