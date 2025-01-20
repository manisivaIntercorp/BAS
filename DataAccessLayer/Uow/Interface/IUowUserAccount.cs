using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Uow.Interface
{
    public interface IUowUserAccount : IDisposable
    {
        IUserAccountDAL UserAccountDALRepo { get; }
        void Commit();
    }
}
