using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IAuditLogDAL
    {

        Task<bool> LogAudit(AuditLog auditLog);
    }
}
