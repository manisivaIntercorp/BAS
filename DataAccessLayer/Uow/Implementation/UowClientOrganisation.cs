using DataAccessLayer.Implementation;
using DataAccessLayer.Interface;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowClientOrganisation : IUowClientOrganisation, IDisposable
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbTransaction _transaction;
        private readonly DbConnection _connection;
        private bool _disposedValue = false;

        public UowClientOrganisation(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            // Fetch the latest connection string dynamically
            string connectionString = GetConnectionStringFromContext();

            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        private string GetConnectionStringFromContext()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null && context.Items.ContainsKey("connection"))
            {
                return context.Items["connection"]?.ToString();
            }

            throw new Exception("Connection string not found in HttpContext.");
        }

        public IClientOrganisationDAL ClientOrganisationDALRepo
        {
            get
            {
                return new ClientOrganisationDAL(_transaction, GetConnectionStringFromContext());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _connection?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void commit()
        {
            try
            {
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
                throw; // Ensure the caller is aware of the issue
            }
            finally
            {
                Dispose();
            }
        }

        ~UowClientOrganisation()
        {
            Dispose(false);
        }
    }

    //public class UowClientOrganisation :  IUowClientOrganisation, IDisposable
    //{
    //    private IClientOrganisationDAL? _objClientOrganisationDAL = null;
    //    private readonly Func<string> _getConnectionString;
    //    private readonly DbTransaction _transaction;
    //    private readonly DbConnection _connection;
    //    private bool _disposedValue = false;
    //    private readonly string connectionString;

    //    //public UowClientOrganisation(Func<string> getConnectionString)
    //    //{
    //    //    _getConnectionString = getConnectionString ?? throw new ArgumentNullException(nameof(getConnectionString), "Connection string function cannot be null.");

    //    //    // Fetch the latest connection string dynamically
    //    //    string connectionString = _getConnectionString();

    //    //    _connection = new SqlConnection(connectionString);
    //    //    _connection.Open();
    //    //    _transaction = _connection.BeginTransaction();
    //    //}

    //    public UowClientOrganisation(string connectionString)
    //    {
    //        if (string.IsNullOrWhiteSpace(connectionString))
    //            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");

    //        this.connectionString = connectionString;

    //        // Initialize connection and transaction
    //        _connection = new SqlConnection(connectionString);
    //        _connection.Open();
    //        _transaction = _connection.BeginTransaction();
    //    }

    //    public IClientOrganisationDAL ClientOrganisationDALRepo
    //    {
    //        get
    //        {
    //            // Lazy initialization of OrganisationDAL
    //            return _objClientOrganisationDAL ??= new ClientOrganisationDAL(_transaction, connectionString);
    //        }

    //    }



    //    /// <summary>
    //    /// Disposes the Unit of Work resources.
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (!_disposedValue)
    //        {
    //            if (disposing)
    //            {
    //                DisposeTransaction();
    //                DisposeConnection();
    //            }

    //            _disposedValue = true;
    //        }
    //    }

    //    private void DisposeTransaction()
    //    {
    //        _transaction?.Dispose();
    //    }

    //    private void DisposeConnection()
    //    {
    //        if (_connection != null && _connection.State == ConnectionState.Open)
    //        {
    //            _connection.Dispose();
    //        }
    //    }

    //    public void commit()
    //    {
    //        try
    //        {
    //            _transaction?.Commit();
    //        }
    //        catch
    //        {
    //            _transaction?.Rollback();
    //            throw; // Ensure the caller is aware of the issue
    //        }
    //        finally
    //        {
    //            DisposeTransaction();
    //        }
    //    }

    //    /// <summary>
    //    /// Destructor to ensure resources are released.
    //    /// </summary>
    //    ~UowClientOrganisation()
    //    {
    //        Dispose(false);
    //    }
    //}
}
