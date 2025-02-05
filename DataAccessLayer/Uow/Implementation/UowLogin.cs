using System.Collections.Generic;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using DataAccessLayer.Uow.Interface;

using System;
using System.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowLogin : IUowLogin, IDisposable
    {
        private ILoginDAL? objLoginDal = null;
        private readonly IDbTransaction transaction;
        private readonly IDbConnection? connection = null;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Existing constructor that accepts connection string and IHttpContextAccessor.
        public UowLogin(string connectionstring, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor.HttpContext?.Session.GetString("DBName") != null)
            {
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("DBName") ?? "";
                string finalConnectionString = BuildConnectionString(connectionstring, dbName);
                connection = new SqlConnection(finalConnectionString);
                connection.Open();
                transaction = connection.BeginTransaction();
            }
            else if (_httpContextAccessor.HttpContext?.Session != null &&
                     _httpContextAccessor.HttpContext?.Session.GetString("InstanceName") != null &&
                     _httpContextAccessor.HttpContext?.Session.GetString("InstanceChange") == "Y" &&
                     _httpContextAccessor.HttpContext?.Session.GetString("DataBaseUserName") != null &&
                     _httpContextAccessor.HttpContext?.Session.GetString("DataBasePassword") != null)
            {
                string serverName = _httpContextAccessor.HttpContext?.Session?.GetString("InstanceName") ?? "";
                string userId = _httpContextAccessor.HttpContext?.Session?.GetString("DataBaseUserName") ?? "";
                string password = _httpContextAccessor.HttpContext?.Session?.GetString("DataBasePassword") ?? "";
                string dbName = _httpContextAccessor.HttpContext?.Session?.GetString("DBName") ?? "";
                string maxPoolSize = _httpContextAccessor.HttpContext?.Session?.GetString("MaxPoolSize") ?? "100";

                string finalConnectionString = BuildConnectionString(connectionstring, serverName, userId, password, dbName, maxPoolSize);
                connection = new SqlConnection(finalConnectionString);
                connection.Open();
                transaction = connection.BeginTransaction();
            }
            else
            {
                connection = new SqlConnection(connectionstring);
                connection.Open();
                transaction = connection.BeginTransaction();
            }
        }

        // Overloaded constructor that retrieves the connection string from HttpContext (set by your middleware)
        public UowLogin(IHttpContextAccessor httpContextAccessor): this(
           // Try to get the connection string from HttpContext.Items.
           httpContextAccessor.HttpContext?.Items["connection"] as string
           // If not found, fallback to the configuration.
           ?? (httpContextAccessor.HttpContext?.RequestServices
                 .GetService(typeof(IConfiguration)) as IConfiguration)
                 ?.GetConnectionString("connection")
           // If still not found, throw an exception.
           ?? throw new InvalidOperationException("No connection string found in HttpContext or configuration."),
           httpContextAccessor)
        {
        }
        public ILoginDAL LoginDALRepo
        {
            get
            {
                return objLoginDal == null ? objLoginDal = new LoginDAL(transaction) : objLoginDal;
            }
        }

        private string BuildConnectionString(string baseConnectionString, string dbName)
        {
            var builder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = dbName
            };
            return builder.ToString();
        }

        private string BuildConnectionString(string baseConnectionString, string serverName, string userID, string password, string dbName, string maxPoolSize)
        {
            var builder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                DataSource = serverName,
                UserID = userID,
                Password = password,
                InitialCatalog = dbName,
                //MaxPoolSize = int.Parse(maxPoolSize)
            };
            return builder.ToString();
        }

        public void commit()
        {
            try
            {
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
                objLoginDal = null;
            }
        }

        private bool disposedValue = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    transaction?.Dispose();
                    connection?.Dispose();
                }
                disposedValue = true;
            }
        }

        ~UowLogin()
        {
            Dispose(false);
        }
    }
}
