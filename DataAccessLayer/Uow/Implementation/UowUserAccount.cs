using DataAccessLayer.Uow.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
//using System.Data.SqlClient;
//using Microsoft.Extensions.Configuration;
namespace DataAccessLayer.Uow.Implementation
{
    public class UowUserAccount: IUowUserAccount
    {
        IUserAccountDAL? objUserAccountDAL = null;
        IDbTransaction transaction;
        IDbConnection? connection = null;

        public UowUserAccount(string connnectionstring)
        {
            connection = new System.Data.SqlClient.SqlConnection(connnectionstring);
            connection.Open();
            transaction = connection.BeginTransaction();
        }
        public IUserAccountDAL UserAccountDALRepo
        {
            get
            {
                return objUserAccountDAL == null ? objUserAccountDAL = new UserAccountDAL(transaction) : objUserAccountDAL;
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

                objUserAccountDAL = null;
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
        ~UowUserAccount()
        {
            Dispose(false);
        }
    }
}
