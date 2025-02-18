using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using DataAccessLayer.Uow.Interface;
using DataAccessLayer.Services;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;


namespace DataAccessLayer.Uow.Implementation
{
    public class UowAuditLog : IUowAuditLog
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly IAuditLogDAL _auditLogDAL;

        public UowAuditLog(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("connection")
                                   ?? throw new ArgumentNullException("Connection string is missing.");

            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            _auditLogDAL = new AuditLogDAL(_transaction, connectionString);
        }

        public IAuditLogDAL AuditLogDALRepo => _auditLogDAL;

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _connection.Dispose();
            }
        }
    }
}

