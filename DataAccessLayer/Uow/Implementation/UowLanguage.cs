using DataAccessLayer.Uow.Interface;
using System.Data;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowLanguage: IUowLanguage
    {
        ILanguageDAL? objLanguageDAL = null;
        IDbTransaction transaction;
        IDbConnection? connection = null;
        public UowLanguage(string connnectionstring)
        {
            connection = new System.Data.SqlClient.SqlConnection(connnectionstring);
            connection.Open();
            transaction = connection.BeginTransaction();
        }
        public ILanguageDAL LanguageDALRepo
        {
            get
            {
                return objLanguageDAL == null ? objLanguageDAL = new LanguageDAL(transaction) : objLanguageDAL;
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

                objLanguageDAL = null;
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
        ~UowLanguage()
        {
            Dispose(false);
        }
    }
}
