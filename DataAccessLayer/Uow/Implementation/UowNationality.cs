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

namespace DataAccessLayer.Uow.Implementation
{
    public class UowNationality : IUowNationality

    {
      //  private readonly IConfiguration _configuration;
        INationalityDAL? objNationalityDAL = null;
        IDbTransaction transaction;
        IDbConnection? connection = null;
        public UowNationality(string connnectionstring)
        {
            connection = new SqlConnection(connnectionstring);
            connection.Open();
            transaction = connection.BeginTransaction();
        }
        public INationalityDAL NationalityDALRepo
        {
            get { 
               return objNationalityDAL == null? objNationalityDAL = new NationalityDAL(transaction) : objNationalityDAL;
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
            finally { 
                 transaction.Dispose();
               
                objNationalityDAL = null;
            }
        }
        private bool disposedValue=false;
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
        ~UowNationality()
        {
            Dispose(false);
        }
    }

   
    }

