using DataAccessLayer.Implementation;
using DataAccessLayer.Interface;
using DataAccessLayer.Uow.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowEmailTemplate: IUowEmailTemplate
    {
        EmailTemplateDAL? objEmailTemplateDAL = null;
        IDbTransaction _transaction;
        
        private readonly string connectionString;
        private readonly IDbConnection _connection;
        private bool _disposedValue = false; // To detect redundant calls
        IDbConnection? connection = null;
        
        public UowEmailTemplate(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");

            this.connectionString = connectionString;

            // Initialize connection and transaction
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public EmailTemplateDAL EmailTemplateDALRepo
        {
            get
            {
                return objEmailTemplateDAL == null ? objEmailTemplateDAL = new EmailTemplateDAL(_transaction, connectionString) : objEmailTemplateDAL;
            }
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
            }
            finally
            {
                _transaction.Dispose();

                objEmailTemplateDAL = null;
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
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        //  transaction = null;
                    }
                    if (connection != null)
                    {
                        connection.Dispose();
                        connection = null;
                    }
                }
                disposedValue = true;
            }
        }
        ~UowEmailTemplate()
        {
            Dispose(false);
        }
    }
}
