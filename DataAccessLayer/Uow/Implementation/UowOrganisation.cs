using DataAccessLayer.Uow.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowOrganisation : IUowOrganisation, IDisposable
    {
        private IOrganisationDAL? _objOrganisationDAL = null;
        private readonly IDbTransaction _transaction;
        private readonly string connectionString;
        private readonly IDbConnection _connection;
        private bool _disposedValue = false; // To detect redundant calls

        //public UowOrganisation(string connectionString)
        //{
        //    // Initialize connection and transaction
        //    _connection = new System.Data.SqlClient.SqlConnection(connectionString);
        //    _connection.Open();
        //    _transaction = _connection.BeginTransaction();
        //}


        public UowOrganisation(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");

            this.connectionString = connectionString;

            // Initialize connection and transaction
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public IOrganisationDAL OrganisationDALRepo
        {
            get
            {
                // Lazy initialization of OrganisationDAL
                return _objOrganisationDAL ??= new OrganisationDAL(_transaction, connectionString);
            }
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void Commit()
        {
            try
            {
                _transaction?.Commit();
            }
            catch
            {
                // Rollback transaction if commit fails
                _transaction?.Rollback();
                throw; // Rethrow exception to ensure the caller is aware of the issue
            }
            finally
            {
                // Clean up resources after commit or rollback
                DisposeTransaction();
            }
        }

        /// <summary>
        /// Disposes the Unit of Work resources.
        /// </summary>
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
                    // Dispose managed resources
                    DisposeTransaction();
                    DisposeConnection();
                }

                _disposedValue = true;
            }
        }

        private void DisposeTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
        }

        private void DisposeConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Dispose();
            }
        }

        /// <summary>
        /// Destructor to ensure resources are released.
        /// </summary>
        ~UowOrganisation()
        {
            Dispose(false);
        }
    }
}

