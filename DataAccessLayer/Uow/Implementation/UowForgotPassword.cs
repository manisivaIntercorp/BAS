using DataAccessLayer.Uow.Interface;
using System.Data;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using System.Globalization;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowForgotPassword: IUowForgotPassword
    {
        IForgotPasswordDAL? objForgotPasswordDAL = null;
        IDbTransaction transaction;
        private readonly string connectionString;
        IDbConnection? connection = null;
        public UowForgotPassword(string connnectionstring) {
            connection = new System.Data.SqlClient.SqlConnection(connnectionstring);
            connection.Open();
            transaction = connection.BeginTransaction();
        }

        public IForgotPasswordDAL ForgotPasswordDALRepo {
            get
            {
                return objForgotPasswordDAL == null ? objForgotPasswordDAL = new ForgotPasswordDAL(transaction,connectionString) : objForgotPasswordDAL;
            }
        }

        public void Commit()
        {
            try
            {
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();

                objForgotPasswordDAL = null;
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
                    if (transaction != null)
                    {
                        transaction.Dispose();
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
        ~UowForgotPassword()
        {
            Dispose(false);
        }
    }
}
