using DataAccessLayer.Implementation;
using DataAccessLayer.Interface;
using DataAccessLayer.Uow.Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowMenu : IUowMenu, IDisposable
    {
        private IMenuDAL? _objMenuDAL = null;
        private readonly IDbTransaction _transaction;
        private readonly string connectionString;
        private readonly IDbConnection _connection;
        private bool _disposedValue = false;

        public UowMenu(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");

            this.connectionString = connectionString;

            // Initialize connection and transaction
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public IMenuDAL MenuDALRepo
        {
            get
            {
                // Lazy initialization of OrganisationDAL
                return _objMenuDAL ??= new MenuDAL(_transaction, connectionString);
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
        ~UowMenu()
        {
            Dispose(false);
        }
    }
}
