using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class RepositoryBase
    {
        public IDbTransaction Transaction { get; private set; }
        public IDbConnection? Connection { get; private set; }
        public RepositoryBase(IDbTransaction _transaction) { 
           Transaction = _transaction;
            Connection = _transaction.Connection;
        }
    }
}
