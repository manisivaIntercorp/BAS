using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using DataAccessLayer.Uow.Interface;
//using System.Data.SqlClient;
//using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowRole : IUowRole
    {
        IRoleDAL? objRoleDAL = null;
        IDbTransaction transaction;
        IDbConnection? connection = null;
        public UowRole(string connnectionstring)
        {
            connection = new System.Data.SqlClient.SqlConnection(connnectionstring);
            connection.Open();
            transaction = connection.BeginTransaction();
        }
        public IRoleDAL RoleDALRepo
        {
            get
            {
                return objRoleDAL == null ? objRoleDAL = new RoleDAL(transaction) : objRoleDAL;
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

                objRoleDAL = null;
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
        ~UowRole ()
        {
            Dispose(false);
        }
    }
}
