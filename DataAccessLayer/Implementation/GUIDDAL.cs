using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Services;
using System.Data;
using Microsoft.Data.SqlClient;
using DataAccessLayer.Model;

namespace DataAccessLayer.Implementation
{
    public class GUIDDAL: RepositoryBase, IGUIDDAL
    {
        private readonly IDbTransaction? _transaction;
        
        private readonly string? _connectionString;
        public GUIDDAL(IDbTransaction? transaction, string? connectionString) : base(transaction)
        {
            _connectionString = connectionString;
            _transaction = transaction;
        }
        public async Task<(bool GetGuid, int RetVal, string Msg)> GetGUID(string? UserGuid)
        {
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
