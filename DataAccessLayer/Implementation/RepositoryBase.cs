using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccessLayer.Implementation
{
    public class RepositoryBase
    {
        public IDbTransaction? Transaction { get; private set; }
        public IDbConnection Connection { get; set; }
        
        public RepositoryBase(IDbTransaction? _transaction) { 
           Transaction = _transaction ?? throw new ArgumentNullException(nameof(_transaction));
            Connection = _transaction.Connection ?? throw new InvalidOperationException("Transaction's Connection is null."); ;
        }
        public RepositoryBase(IDbConnection? connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));

        }
    }
}
