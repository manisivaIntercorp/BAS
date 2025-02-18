using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccessLayer.Implementation
{
    public class AuditLogDAL : IAuditLogDAL
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;

        public AuditLogDAL(IDbTransaction transaction, string connectionString)
        {
            _transaction = transaction;
            _connection = transaction.Connection ?? throw new ArgumentNullException(nameof(transaction.Connection));
        }

        public async Task<bool> LogAudit(AuditLog auditLog)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserId", auditLog.UserId);
                parameters.Add("@UserGuid", auditLog.UserGuid);
                parameters.Add("@Token", auditLog.Token);
                parameters.Add("@Action", auditLog.Action);
                parameters.Add("@IPAddress", auditLog.IPAddress);
                parameters.Add("@DeviceInfo", auditLog.DeviceInfo);
                parameters.Add("@CreatedBy", auditLog.CreatedBy);
                parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

                if (_connection == null)
                    throw new ArgumentNullException(nameof(_connection), "Database connection cannot be null.");

                await _connection.ExecuteAsync(
                    "sp_LogAudit",
                    parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure
                );

                int result = parameters.Get<int>("@RetVal");
                string errorMessage = parameters.Get<string>("@ErrorMessage");

                if (result > 0)
                {
                    _transaction.Commit(); // Commit the transaction
                    return true;
                }
                else
                {
                    _transaction.Rollback(); // Rollback on failure
                    Console.WriteLine($"Audit Log Error: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _transaction.Rollback();
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }
    }
}
