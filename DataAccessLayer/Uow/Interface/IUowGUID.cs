using DataAccessLayer.Implementation;
using DataAccessLayer.Model;

namespace DataAccessLayer.Uow.Interface
{
     public interface IUowGUID : IDisposable
    {
        public GUIDDAL GUIDDALRepo { get; }
        void Commit();

    }
}
