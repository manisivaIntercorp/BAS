using System.Collections.Generic;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using DataAccessLayer.Uow.Interface;

using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.SqlServer.Management.XEvent;
using Microsoft.SqlServer.Management.Smo;
using System.Xml.Linq;
using DataAccessLayer.Services;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowLogin : IUowLogin, IDisposable
    {
        private ILoginDAL? objLoginDal = null;
        private readonly IDbTransaction transaction;
        private readonly IDbConnection? connection = null;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly EncryptedDecrypt _encryptedDecrypt;

        public UowLogin(string connectionString, IHttpContextAccessor httpContextAccessor, IConfiguration configuration,EncryptedDecrypt encryptedDecrypt)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _encryptedDecrypt = encryptedDecrypt;

            if (!string.IsNullOrEmpty(connectionString))
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                transaction = connection.BeginTransaction();
            }
            else
            {
                throw new ArgumentException("Invalid connection string.");
            }
        }
        public UowLogin(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, EncryptedDecrypt encryptedDecrypt)
     : this(
         GetConnectionString(httpContextAccessor, configuration),
         httpContextAccessor,
         configuration,
         encryptedDecrypt)
        {
        }

        //// Overloaded constructor that retrieves the connection string from HttpContext (set by your middleware)
        //public UowLogin(IHttpContextAccessor httpContextAccessor): this(
        //   // Try to get the connection string from HttpContext.Items.
        //   httpContextAccessor.HttpContext?.Items["connection"] as string
        //   // If not found, fallback to the configuration.
        //   ?? (httpContextAccessor.HttpContext?.RequestServices
        //         .GetService(typeof(IConfiguration)) as IConfiguration)
        //         ?.GetConnectionString("connection")
        //   // If still not found, throw an exception.
        //   ?? throw new InvalidOperationException("No connection string found in HttpContext or configuration."),httpContextAccessor)
        //{
        //}
        public ILoginDAL LoginDALRepo
        {
            get
            {
                return objLoginDal == null ? objLoginDal = new LoginDAL(transaction, _httpContextAccessor,_configuration,_encryptedDecrypt) : objLoginDal;
            }
        }
        private static string GetConnectionString(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            var connectionString = httpContextAccessor.HttpContext?.Items["connection"] as string;

            return connectionString ?? configuration.GetConnectionString("connection")
                ?? throw new InvalidOperationException("No connection string found.");
        }
        private string BuildConnectionString(string baseConnectionString, string dbName)
        {
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = dbName
            };
            return builder.ToString();
        }

        private string BuildConnectionString(string baseConnectionString, string serverName, string userID, string password, string dbName, string maxPoolSize)
        {
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(baseConnectionString)
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
            catch (Exception)
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
