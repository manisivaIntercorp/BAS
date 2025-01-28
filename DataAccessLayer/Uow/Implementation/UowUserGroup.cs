using DataAccessLayer.Uow.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowUserGroup: IUowUserGroup
    {
        IUserGroupDAL? objUserGroupDAL = null;
        IDbTransaction transaction;
        IDbConnection? connection = null;
        public UowUserGroup(string connnectionstring) {
            connection = new System.Data.SqlClient.SqlConnection(connnectionstring);
            connection.Open();
            transaction = connection.BeginTransaction();
        }
        public IUserGroupDAL UserGroupDALRepo
        {
            get
            {
                return objUserGroupDAL == null ? objUserGroupDAL = new UserGroupDAL(transaction) : objUserGroupDAL;
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

                objUserGroupDAL = null;
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
        ~UowUserGroup()
        {
            Dispose(false);
        }
    }
}
