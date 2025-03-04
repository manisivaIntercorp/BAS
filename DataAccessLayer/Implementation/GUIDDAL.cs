using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Services;
using System.Data;
using Microsoft.Data.SqlClient;
using DataAccessLayer.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.Implementation
{
    public class GUIDDAL: RepositoryBase, IGUIDDAL
    {
        private readonly IDbTransaction? _transaction;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string? _connectionString;
        public GUIDDAL(IDbTransaction? transaction, string? connectionString, IConfiguration configuration) : base(transaction)
        {
            _connectionString = connectionString;
            _transaction = transaction;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public GUIDDAL(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(new SqlConnection(configuration.GetConnectionString("connection"))) // ✅ Always Start with Master DB
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            
            SetDynamicConnection();
        }
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
        
        public async Task<(bool GetGuid, int RetVal, string Msg)> GetGUID(string? UserGuid)
        {
            if (Connection.Database == "master")
            {
                string? masterConnection = _configuration.GetConnectionString("connection");
                Connection = new SqlConnection(masterConnection);
            }
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "GET_USERGUID");
            parameters.Add("@UpdatedBy", UserGuid);
            parameters.Add("@RetVal", dbType: DbType.Int32,direction:ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_GetGUID",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var guid = (await multi.ReadAsync<GUIDModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }
            bool res= guid.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            return (res, RetVal, Msg);
        }

        public async Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnUserAccountRoleGuid(string? UserGuid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "GET_USERAccountRoleGUID");
            parameters.Add("@UpdatedBy", UserGuid);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_GetGUID",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var guid = (await multi.ReadAsync<GUIDModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }
            bool res = guid.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            return (res, RetVal, Msg);
        }

        public async Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnRoleGuid(string? UserGuid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "GET_ROLEGUID");
            parameters.Add("@UpdatedBy", UserGuid);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_GetGUID",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var guid = (await multi.ReadAsync<GUIDModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }
            bool res = guid.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            return (res, RetVal, Msg);
        }

        public async Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnOrgName(string? UserGuid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "GET_ORGGUID");
            parameters.Add("@UpdatedBy", UserGuid);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_GetGUID",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var guid = (await multi.ReadAsync<GUIDModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }
            bool res = guid.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            return (res, RetVal, Msg);
        }

        public async Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnUserPolicy(string? UserGuid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "GET_UserPolicyGUID");
            parameters.Add("@UpdatedBy", UserGuid);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_GetGUID",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var guid = (await multi.ReadAsync<GUIDModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }
            bool res = guid.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            return (res, RetVal, Msg);
        }

        public async Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnNationality(string? UserGuid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "GET_NationalityGUID");
            parameters.Add("@UpdatedBy", UserGuid);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_GetGUID",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var guid = (await multi.ReadAsync<GUIDModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }
            bool res = guid.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            return (res, RetVal, Msg);
        }

        public async Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnMailServer(string? updatedGuidBy)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", "GET_MailServerGUID");
            parameters.Add("@UpdatedBy", updatedGuidBy);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_GetGUID",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var guid = (await multi.ReadAsync<GUIDModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }
            bool res = guid.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            return (res, RetVal, Msg);
        }
    }
}
