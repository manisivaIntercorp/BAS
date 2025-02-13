using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using DataAccessLayer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Management.XEvent;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Net.Http;


namespace DataAccessLayer.Implementation
{
    public class LoginDAL : RepositoryBase, ILoginDAL
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly EncryptedDecrypt? _encryptedDecrypt;
        public LoginDAL(IDbTransaction transaction, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, EncryptedDecrypt encryptedDecrypt ) : base(transaction)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _encryptedDecrypt = new EncryptedDecrypt(configuration);
        }

        public async Task<List<ResultModel>> UserLogin(LoginModel objLoginModel)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Mode", "Get");
            dynamicParameters.Add("@UserName", objLoginModel.UserName);
            dynamicParameters.Add("@Password", objLoginModel.Password);
            dynamicParameters.Add("@UserType", objLoginModel.UserType);
            dynamicParameters.Add("@IPAddress", objLoginModel.IPAddress);
            dynamicParameters.Add("@DeviceName", objLoginModel.DeviceName);
            dynamicParameters.Add("@BrowserName", objLoginModel.BrowserName);

            if (Connection == null)
                throw new ArgumentNullException(nameof(Connection), "The database connection cannot be null.");

 

            using var multi = await Connection.QueryMultipleAsync(
                "sp_Authentication",
                dynamicParameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);

            var res = multi.Read<ResultModel>().ToList();

            if (res is { Count: > 0 })
            {
                switch (res[0].RetVal)
                {
                    case -1:
                        return res;

                    case 1:
                        var details = multi.Read<LoginDetailModel>().ToList();
                        res[0].lstLoginDetails = details;
                        break;
                }
            }



            return res;
        }

        public async Task<List<ResultModel>> ClientUserLogin(LoginModel objLoginModel)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Mode", "Get");
            dynamicParameters.Add("@UserName", objLoginModel.UserName);
            dynamicParameters.Add("@Password", objLoginModel.Password);
            dynamicParameters.Add("@UserType", objLoginModel.UserType);
            dynamicParameters.Add("@IPAddress", objLoginModel.IPAddress);
            dynamicParameters.Add("@DeviceName", objLoginModel.DeviceName);
            dynamicParameters.Add("@BrowserName", objLoginModel.BrowserName);

            string connectionString = GetConnectionString(Connection.ConnectionString);

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(
                "sp_Authentication",
                dynamicParameters,
                commandType: CommandType.StoredProcedure);

            var res = multi.Read<ResultModel>().ToList();

            if (res.Any() && res[0].RetVal == 1)
            {
                var details = multi.Read<LoginDetailModel>().ToList();
                res[0].lstLoginDetails = details;
            }

            return res;
        }


        private string GetConnectionString(string OldConnectionstring)
        {
            string? connectionString = string.Empty;
            var session = _httpContextAccessor.HttpContext.Session;

            if (session != null && !string.IsNullOrEmpty(session.GetString("DBName")) &&
                session.GetString("InstanceChange") != "Y")
            {
                connectionString = BuildConnectionString(session.GetString("DBName"));
            }
            else if (session != null &&
                     session.GetString("InstanceName") != null &&
                     session.GetString("InstanceChange") == "Y" &&
                     session.GetString("DataBaseUserName") != null &&
                     session.GetString("DataBasePassword") != null)
            {
                connectionString = BuildConnectionString(
                    _encryptedDecrypt.Decrypt(session.GetString("InstanceName")),
                    _encryptedDecrypt.Decrypt(session.GetString("DataBaseUserName")),
                    _encryptedDecrypt.Decrypt(session.GetString("DataBasePassword")),
                    session.GetString("DBName"));
            }
            else
            {
                // var config = context.RequestServices.GetService<IConfiguration>();
                connectionString = OldConnectionstring;

            }


            return connectionString;
        }

        private string BuildConnectionString(string? dbName)
        {
            var config = _httpContextAccessor?.HttpContext?.RequestServices.GetService<IConfiguration>();
            var connectionString = config?.GetConnectionString("connection");
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = dbName
            };
            return builder.ToString();
        }

        private string BuildConnectionString(string? serverName, string? userID, string? password, string? dbName)
        {
            var config = _httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var connectionString = config.GetConnectionString("connection");
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                DataSource = serverName,
                UserID = userID,
                Password = password,
                InitialCatalog = dbName,
                TrustServerCertificate = true,
                MultipleActiveResultSets = true
            };
            return builder.ToString();
        }
        public async Task<List<ResultModel>> GetUserID(LoginModel objloginModel)
        {
            DynamicParameters dyParameter = new DynamicParameters();
            dyParameter.Add("@Mode", "GET_USER_ID");
            dyParameter.Add("@UserName", objloginModel.UserName);
            dyParameter.Add("@Password", objloginModel.Password);
            dyParameter.Add("@UserType", objloginModel.UserType);
            dyParameter.Add("@IPAddress", objloginModel.IPAddress);
            dyParameter.Add("@DeviceName", objloginModel.DeviceName);
            dyParameter.Add("@BrowserName", objloginModel.BrowserName);
            if (Connection == null)
                throw new ArgumentNullException(nameof(Connection), "The database connection cannot be null.");

            var multi = await Connection.QueryMultipleAsync("sp_Authentication",
                dyParameter,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<ResultModel>().ToList();
            if (res is { Count: > 0 })
            {
                switch (res[0].RetVal)
                {
                    case -1:
                        return res;

                    case 1:
                        var details = multi.Read<LoginDetailModel>().ToList();
                        res[0].lstLoginDetails = details;
                        break;
                }
            }


            return res;
        }

        public async Task<List<OrganisationDBDetails>> GetOrganisationWithDBDetails(LoginModel objloginModel)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Mode", "GET_ORG");
            dynamicParameters.Add("@UserName", objloginModel.UserName);
            if (Connection == null)
                throw new ArgumentException(nameof(Connection), "The database connection cannot be null.");

            var multi = await Connection.QueryMultipleAsync("sp_Authentication",
                dynamicParameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<OrganisationDBDetails>().ToList();
            return res;
        }

        public async Task<List<GetDropDownDataModel>> GetDDlLanguage(string Mode, string RefID1, string RefID2, string RefID3)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Mode", Mode);
            dynamicParameters.Add("RefID1", RefID1);
            dynamicParameters.Add("RefID2", RefID2);
            dynamicParameters.Add("RefID3", RefID3);
            if (Connection == null)
                throw new ArgumentException(nameof(Connection), "The database connection cannot be null.");

            var multi = await Connection.QueryMultipleAsync("sp_ListData",
                dynamicParameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<GetDropDownDataModel>().ToList();
            return res;
        }

        public async Task<List<GetDropDownDataModel>> GetDDlModule(string Mode, string RefID1, string RefID2, string RefID3)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Mode", Mode);
            dynamicParameters.Add("RefID1", RefID1);
            dynamicParameters.Add("RefID2", RefID2);
            dynamicParameters.Add("RefID3", RefID3);
            if (Connection == null)
                throw new ArgumentException(nameof(Connection), "The database connection cannot be null.");

            var multi = await Connection.QueryMultipleAsync("sp_ListData",
                dynamicParameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<GetDropDownDataModel>().ToList();
            return res;
        }
    }
}
