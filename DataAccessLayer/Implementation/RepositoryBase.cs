using System.Data;

namespace DataAccessLayer.Implementation
{
    public class RepositoryBase
    {
        public IDbTransaction? Transaction { get; private set; }
        public IDbConnection Connection { get; private set; }
        public RepositoryBase(IDbTransaction? _transaction) { 
           Transaction = _transaction ?? throw new ArgumentNullException(nameof(_transaction));
            Connection = _transaction.Connection ?? throw new InvalidOperationException("Transaction's Connection is null."); ;
        }
    }
}
